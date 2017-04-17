using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;

using Accord;
using Accord.Math;
using Accord.Statistics;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Linear;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.DecisionTrees;

namespace MachineLearning_Test
{
    class AlphaModel
    {
    }

    class DataSetting
    {
        List<string> companies = new List<string>();
        
        // 입력형식 : tableName -> SK이노베이션, dateStart -> '2016-01-01'
        double[] trainPrepare(string tableName, string dateStart, string dateEnd)
        {
            string connectionString = @"Data Source =.\SQLEXPRESS; Initial Catalog = Stock_Data; Integrated Security = True; Connect Timeout = 15; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
            List<double> temp = new List<double>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("getStockData", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("@table_id", System.Data.SqlDbType.VarChar, 200);
                    param.Value = tableName;
                    command.Parameters.Add(param);
                    param = new SqlParameter("@dateStart", System.Data.SqlDbType.VarChar, 50);
                    param.Value = dateStart;
                    command.Parameters.Add(param);
                    param = new SqlParameter("@dateEnd", System.Data.SqlDbType.VarChar, 50);
                    param.Value = dateEnd;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        temp.Add(Convert.ToDouble(dataReader["PRICE_CLOSE"].ToString()));
                    }
                    connection.Close();
                }
                catch
                {
                    throw;
                }
            }
            double[] priceSet = temp.ToArray();

            return priceSet;
        }

        double[] testPrepare(string tableName, string dateStart, string dateEnd)
        {
            string connectionString = @"Data Source =.\SQLEXPRESS; Initial Catalog = Stock_Data; Integrated Security = True; Connect Timeout = 15; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
            List<double> temp = new List<double>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("getStockData", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("@table_id", System.Data.SqlDbType.VarChar, 200);
                    param.Value = tableName;
                    command.Parameters.Add(param);
                    param = new SqlParameter("@dateStart", System.Data.SqlDbType.VarChar, 50);
                    param.Value = dateStart;
                    command.Parameters.Add(param);
                    param = new SqlParameter("@dateEnd", System.Data.SqlDbType.VarChar, 50);
                    param.Value = dateEnd;
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        temp.Add(Convert.ToDouble(dataReader["PRICE_CLOSE"].ToString()));
                    }
                    connection.Close();
                }
                catch
                {
                    throw;
                }
            }
            double[] priceSet = temp.ToArray();

            return priceSet;
        }

        string downloadCode()
        {
            string url = "http://datamall.koscom.co.kr/servlet/infoService/SearchIssue";
            using (WebClient client = new WebClient())
            {
                //한글깨짐 인코딩 작업
                Encoding encKr = Encoding.GetEncoding("euc-kr");
                EncodingInfo[] encods = Encoding.GetEncodings();
                Encoding destEnc = Encoding.UTF8;

                byte[] response =
                client.UploadValues(url, new NameValueCollection()
                {
                   { "flag","SEARCH" },
                   { "marketDisabled" , "null" },
                   { "marketBit","1" }
                });
                response = Encoding.Convert(encKr, destEnc, response); //최종 인코딩
                string result = System.Text.Encoding.UTF8.GetString(response); //마크업 스트링 변환                           
                return result;
            }
        }

        // downloadCode() 결과를 parseCodeHtml에 인수로 넣어줘야 한다 !
        void parseCodeHtml(string html)
        {
            Regex firstRegex = new Regex("<option value=\"KR.*\">.*?</option>");
            Match firstMatch = firstRegex.Match(html);
            MatchCollection firstCollection = firstRegex.Matches(html);
            foreach (Match fc in firstCollection)
            {
                Regex secondRegex = new Regex("<.*?>");
                Regex lastRegex = new Regex(@"<option value=" + "\"" + "|" + "\"" + ">.*?</option>");
                string sr = secondRegex.Replace(fc.Value, "");
                string lr = lastRegex.Replace(fc.Value, "");
                string tr = Regex.Replace(sr, @"\D", ""); // 코드 분류 완료
                sr = Regex.Replace(sr, @"\(.*?\)", ""); // 회사이름 분류 완료               
                if (tr.Length >= 7)
                {
                    tr = tr.Substring(0, 6);
                }
                this.companies.Add(sr); // 회사이름 리스트에 삽입
            }
        }
    }

    class Mean_Reversion
    {
        string[] adfTest(double[] raw_data)
        {
            double[] diff_raw_data = new double[raw_data.Length - 1];
            double[] lag_raw_data = new double[raw_data.Length - 1];
            double[] diff_lag_raw_data = new double[raw_data.Length - 2];

            double[] output_diff = new double[raw_data.Length - 2];
            double[] input_lag = new double[raw_data.Length - 2];
            double[] input_diff_lag = new double[raw_data.Length - 2];
            double[] predicted = new double[raw_data.Length - 2];
            double[][] inputs = new double[raw_data.Length - 2][];
            string critical_values = "error";

            string[] result = new string[3];

            // diff
            for (int i = 0; i < raw_data.Length - 1; i++) diff_raw_data[i] = raw_data[i + 1] - raw_data[i];
            // Lag
            for (int i = 0; i < raw_data.Length - 1; i++) lag_raw_data[i] = raw_data[i];
            // diff_lag
            for (int i = 0; i < raw_data.Length - 2; i++) diff_lag_raw_data[i] = lag_raw_data[i + 1] - lag_raw_data[i];

            // 데이터 세팅
            for (int i = 0; i < raw_data.Length - 2; i++) output_diff[i] = diff_raw_data[i + 1];
            for (int i = 0; i < raw_data.Length - 2; i++) input_lag[i] = lag_raw_data[i + 1];
            for (int i = 0; i < raw_data.Length - 2; i++) input_diff_lag[i] = diff_lag_raw_data[i];

            // 다중 선형 회귀
            OrdinaryLeastSquares olss = new OrdinaryLeastSquares() { UseIntercept = true };
            for (int i = 0; i < raw_data.Length - 2; i++) inputs[i] = new double[] { input_lag[i], input_diff_lag[i] };
            MultipleLinearRegression regression = olss.Learn(inputs, output_diff);
            for (int i = 0; i < raw_data.Length - 2; i++) predicted[i] = regression.Transform(inputs[i]);
            Console.WriteLine(regression.Weights[0] + "\t" + regression.Weights[1]);

            // 음의 t-value보다 작거나 양의 t-value보다 크다면 정상시계열, 그렇지 않다면 랜덤과정이다.
            if ((raw_data.Length - 2) <= 25)
            {
                critical_values = "No trend [5%, -3.00]\tTrend [5%, -3.60]";
            }
            else if ((raw_data.Length - 2) <= 50)
            {
                critical_values = "No trend [5%, -2.93]\tTrend [5%, -3.50]";
            }
            else if ((raw_data.Length - 2) <= 100)
            {
                critical_values = "No trend [5%, -2.89]\tTrend [5%, -3.45]";
            }
            else if ((raw_data.Length - 2) <= 250)
            {
                critical_values = "No trend [5%, -2.88]\tTrend [5%, -3.43]";
            }
            else if ((raw_data.Length - 2) <= 500)
            {
                critical_values = "No trend [5%, -2.87]\tTrend [5%, -3.42]";
            }
            else if ((raw_data.Length - 2) > 500)
            {
                critical_values = "No trend [5%, -2.86]\tTrend [5%, -3.41]";
            }

            result[0] = Calc_T(inputs, output_diff, predicted, regression)[0].ToString();
            result[1] = Calc_T(inputs, output_diff, predicted, regression)[1].ToString();
            result[2] = critical_values;
            return result;
        }

        double[] Calc_T(double[][] input, double[] output, double[] predict, MultipleLinearRegression regression)
        {
            double std1 = 0, std2 = 0, error = 0;
            double xMean1 = 0, xMean2 = 0;
            double temp1 = 0, temp2 = 0;
            double[] tScore = new double[2];

            for (int i = 0; i < output.Length; i++) error += (output[i] - regression.Transform(input)[i]) * (output[i] - regression.Transform(input)[i]);
            error /= output.Length - 3;
            error = Math.Sqrt(error);

            for (int i = 0; i < output.Length; i++) xMean1 += input[i][0];
            xMean1 /= output.Length;
            for (int i = 0; i < output.Length; i++) xMean2 += input[i][1];
            xMean2 /= output.Length;

            for (int i = 0; i < output.Length; i++) std1 += (input[i][0] - xMean1) * (input[i][1] - xMean2);
            std1 = Math.Pow(std1, 2);
            for (int i = 0; i < output.Length; i++) temp1 += (input[i][1] - xMean2) * (input[i][1] - xMean2);
            std1 = std1 / temp1; temp1 = 0;
            for (int i = 0; i < output.Length; i++) temp1 += (input[i][0] - xMean1) * (input[i][0] - xMean1);
            std1 = temp1 - std1;
            std1 = error / Math.Sqrt(std1);

            for (int i = 0; i < output.Length; i++) std2 += (input[i][0] - xMean1) * (input[i][1] - xMean2);
            std2 = Math.Pow(std2, 2);
            for (int i = 0; i < output.Length; i++) temp2 += (input[i][0] - xMean1) * (input[i][0] - xMean1);
            std2 = std2 / temp2; temp2 = 0;
            for (int i = 0; i < output.Length; i++) temp2 += (input[i][1] - xMean2) * (input[i][1] - xMean2);
            std2 = temp2 - std2;
            std2 = error / Math.Sqrt(std2);

            tScore[0] = regression.Weights[0] / std1;
            tScore[1] = regression.Weights[1] / std2;

            return tScore;
        }

        double hurstTest(double[] raw_data)
        {
            double[] devi_raw_data = new double[raw_data.Length];
            double[] sums_devi = new double[raw_data.Length];
            double R = 0;
            double std = 0;
            double mean_raw_data = 0;
            double temp_sums = 0;
            double hurst = 0;

            // 평균 계산
            for (int idx = 0; idx < raw_data.Length; idx++) mean_raw_data += raw_data[idx];
            mean_raw_data /= raw_data.Length;

            // 편차 계산
            for (int idx = 0; idx < raw_data.Length; idx++) devi_raw_data[idx] = raw_data[idx] - mean_raw_data;
            for (int idx = 0; idx < raw_data.Length; idx++)
            {
                temp_sums += devi_raw_data[idx];
                sums_devi[idx] = temp_sums;
            }
            temp_sums = 0;
            Array.Sort(sums_devi);
            R = sums_devi[sums_devi.Length - 1] - sums_devi[0];

            // raw_data의 표준편차 계산
            for (int idx = 0; idx < raw_data.Length; idx++) temp_sums += (raw_data[idx] - mean_raw_data) * (raw_data[idx] - mean_raw_data);
            temp_sums /= raw_data.Length;
            std = Math.Sqrt(temp_sums);

            hurst = Math.Log(R / std) / Math.Log(raw_data.Length);
            return hurst;
        }

        double halflifeTest(double[] raw_data)
        {
            double[] lag_raw_data = new double[raw_data.Length - 1];
            double[] outputs = new double[raw_data.Length - 1];
            double lifeTime;
            double mean_reversion_speed;

            for (int i = 0; i < raw_data.Length - 1; i++) lag_raw_data[i] = raw_data[i + 1];
            for (int i = 0; i < raw_data.Length - 1; i++) outputs[i] = raw_data[i] - lag_raw_data[i];

            //for (int i = 0; i < raw_data.Length - 1; i++)
            //{
            //    Console.Write(lag_raw_data[i] + "\t");
            //}
            //Console.WriteLine();
            //for (int i = 0; i < raw_data.Length - 1; i++)
            //{
            //    Console.Write(raw_data[i] + "\t");
            //}
            //Console.WriteLine();
            //for (int i = 0; i < raw_data.Length - 1; i++)
            //{
            //    Console.Write(outputs[i] + "\t");
            //}
            //Console.WriteLine();
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            SimpleLinearRegression regression = ols.Learn(lag_raw_data, outputs);
            mean_reversion_speed = regression.Slope;
            Console.WriteLine("slope : " + mean_reversion_speed);


            mean_reversion_speed = -1 * Math.Log(1 + mean_reversion_speed);
            Console.WriteLine("mean_reversion_speed : " + mean_reversion_speed);

            lifeTime = (Math.Log(2) / mean_reversion_speed);
            Console.WriteLine("Half Life : " + lifeTime);
            return lifeTime;
        }
    }

    class Machine_Learning
    {
       double[] knnTest(double[] raw_data, double[] raw_test)
        {

            double[][] dataSet = new double[raw_data.Length - 1][];
            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                double temp = raw_data[i];
                dataSet[i] = new double[] { temp };
            }

            double[][] testSet = new double[raw_test.Length][];
            for (int i = 0; i < raw_test.Length; i++)
            {
                double temp = raw_test[i];
                testSet[i] = new double[] { temp };
            }

            int[] outputs = new int[raw_data.Length - 1];
            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                if (raw_data[i + 1] - raw_data[i] < 0)
                {
                    outputs[i] = 0;
                }
                else
                {
                    outputs[i] = 1;
                }
            }

            KNearestNeighbors knn = new KNearestNeighbors(k: 3, classes: 2, inputs: dataSet, outputs: outputs);
            return;
        }

       double[] logisticTest(double[] raw_data, double[] raw_test)
        {
            double[][] dataSet = new double[raw_data.Length - 1][];
            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                double temp = raw_data[i];
                dataSet[i] = new double[] { temp };
            }

            double[][] testSet = new double[raw_test.Length][];
            for (int i = 0; i < raw_test.Length; i++)
            {
                double temp = raw_test[i];
                testSet[i] = new double[] { temp };
            }

            int[] outputs = new int[raw_data.Length - 1];
            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                if (raw_data[i + 1] - raw_data[i] < 0)
                {
                    outputs[i] = 0;
                }
                else
                {
                    outputs[i] = 1;
                }
            }
            //로지스틱이 train 데이터 스케일에 훨씬 더 민감하다.
            var teacher = new ProbabilisticCoordinateDescent()
            {
                Tolerance = 1e-10,
                Complexity = 1e+10,
            };

            var svm = teacher.Learn(dataSet, outputs);

            var regression = (LogisticRegression)svm;
            return;
        }

       double[] randomforestTest(double[] raw_data, double[] raw_test)
        {
            double[][] dataSet = new double[raw_data.Length - 1][];
            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                double temp = raw_data[i];
                dataSet[i] = new double[] { temp };
            }

            double[][] testSet = new double[raw_test.Length][];
            for (int i = 0; i < raw_test.Length; i++)
            {
                double temp = raw_test[i];
                testSet[i] = new double[] { temp };
            }

            int[] outputs = new int[raw_data.Length - 1];
            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                if (raw_data[i + 1] - raw_data[i] < 0)
                {
                    outputs[i] = 0;
                }
                else
                {
                    outputs[i] = 1;
                }
            }

            var teacher = new RandomForestLearning()
            {
                NumberOfTrees = 20,
            };
            var forest = teacher.Learn(dataSet, outputs);
            return;
        }
    }
}
