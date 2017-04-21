using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Data.SqlClient;

using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Linear;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.MachineLearning.DecisionTrees;

namespace MachineLearning_Test
{
    public class AlphaModel
    {
        // 이동평균 입력값과 테스트 데이터의 길이가 같아야함
        public string determinePosition_MA(double [] movingAverage, double [] x_test, int row_idx, bool print)
        {
            double ma_mean = 0, ma_std = 0;
            double price_arbitrage = 0;
            string result = "HOLD";

            for (int idx = 0; idx < movingAverage.Length; idx++) ma_mean += movingAverage[idx];
            ma_mean /= movingAverage.Length;
            for (int idx = 0; idx < movingAverage.Length; idx++) ma_std += Math.Pow((movingAverage[idx] - ma_mean), 2);
            ma_std /= movingAverage.Length;
            ma_std = Math.Sqrt(ma_std);

            price_arbitrage = x_test[row_idx] - movingAverage[row_idx];           

            if(Math.Abs(price_arbitrage) > ma_std * 0.5)
            {
                if(price_arbitrage > 0)
                {
                    result = "SHORT";
                }
                else
                {
                    result = "LONG";
                }
            }

            if (print)
            {
                Console.WriteLine("\ndiff: " + price_arbitrage + "\nprice: " + x_test[row_idx] + "\nmoving_average: "
                    + movingAverage[row_idx] + "\nmoving_average_std: " + ma_std + "\nDecision : " + result);
            }

            return result;
        }

        public string determinePosition_ML(KNearestNeighbors knn, LogisticRegression logistic, Accord.MachineLearning.DecisionTrees.RandomForest forest, double[][] x_test, int row_idx, bool print)
        {
            string result = "HOLD";
            int prediction_result = 0;

            if (row_idx - 1 < 0)
            {
                result = "HOLD";
            }

            prediction_result += knn.Compute(x_test[row_idx]);
            if (logistic.Decide(x_test[row_idx]))
            {
                prediction_result += knn.Compute(x_test[row_idx]);
            }
            prediction_result += forest.Decide(x_test[row_idx]);

            if(prediction_result > 1)
            {
                result = "LONG";
            }else
            {
                result = "SHORT";
            }

            if (print)
            {
                Console.WriteLine("\nknn : " + knn.Compute(x_test[row_idx]) + "\nlogistic : " +logistic.Decide(x_test[row_idx]) + "\nforest : " + forest.Decide(x_test[row_idx]) + "\nDecision : " + result);
            }

            return result;

        }

    }

    public class DataSetting
    {
        public List<string> companies = new List<string>();
        public double[][] x_train;                             // 학습시킬 주가 데이터
        public int[] y_train;                                  // 학습시킬 주가 데이터의 등락 여부
        public double[][] x_test;                              // 테스트할 데이터 셋
        public int[] y_test;                                   // 테스트할 데이터 셋의 등락 여부

        // 입력형식 : tableName -> SK이노베이션, dateStart -> '2016-01-01'
        public double[] getDataPrepare(string tableName, string dateStart, string dateEnd)
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

        // x_train, x_test 데이터 : 위에서 받아온 데이터를 학습에 맞게 변환 
        public double[][] inputDataSetting(double[] train)
        {
            double[][] dataSet = new double[train.Length - 1][];
            for (int i = 0; i < train.Length - 1; i++)
            {
                double temp = train[i];
                dataSet[i] = new double[] { temp };
            }
            return dataSet;
        }

        // y_train, y_test 데이터 : 위에서 받아온 데이터의 등락여부 배열 반환
        public int[] outputDataSetting(double[] train)
        {
            int[] outputs = new int[train.Length - 1];
            for (int i = 0; i < train.Length - 1; i++)
            {
                if (train[i + 1] - train[i] < 0)
                {
                    outputs[i] = 0;
                }
                else
                {
                    outputs[i] = 1;
                }
            }
            return outputs;
        }

        public string downloadCode()
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
        public List<string> parseCodeHtml(string html)
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
            return this.companies;
        }
    }

    public class Mean_Reversion
    {
        public string[] adfTest(double[] raw_data)
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

        public double[] Calc_T(double[][] input, double[] output, double[] predict, MultipleLinearRegression regression)
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

        public double hurstTest(double[] raw_data)
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

        public double halflifeTest(double[] raw_data)
        {
            double[] lag_raw_data = new double[raw_data.Length - 1];
            double[] outputs = new double[raw_data.Length - 1];
            double lifeTime = 0;
            double mean_reversion_speed;

            for (int i = 0; i < raw_data.Length - 1; i++) lag_raw_data[i] = raw_data[i + 1];
            for (int i = 0; i < raw_data.Length - 1; i++) outputs[i] = raw_data[i] - lag_raw_data[i];

            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            SimpleLinearRegression regression = ols.Learn(lag_raw_data, outputs);
            mean_reversion_speed = regression.Slope;
            // Console.WriteLine("slope : " + mean_reversion_speed);


            mean_reversion_speed = -1 * Math.Log(1 + mean_reversion_speed);
            // Console.WriteLine("mean_reversion_speed : " + mean_reversion_speed);
            try
            {
                lifeTime = (Math.Log(2) / mean_reversion_speed);
            }
            catch
            {
                throw;
            }
           
            //Console.WriteLine("Half Life : " + lifeTime);
            return lifeTime;
        }

        public double[] calcMovingAverage(double[] raw_Data)
        {
            double[] movingAverage = new double[raw_Data.Length - 2];
            for(int idx = 0; idx < raw_Data.Length -2; idx++)
            {
                movingAverage[idx] = (raw_Data[idx] + raw_Data[idx + 1] + raw_Data[idx + 2]) / 3;
            }

            return movingAverage;
        }
    }

    public class Machine_Learning
    {
        public KNearestNeighbors knnTest(double[][] x_train, int[] y_train)
        {   
            KNearestNeighbors knn = new KNearestNeighbors(k: 3, classes: 2, inputs: x_train, outputs: y_train);
            return knn;
        }

        public LogisticRegression logisticTest(double[][] x_train, int[] y_train)
        {
            //로지스틱이 train 데이터 스케일에 훨씬 더 민감하다.
            var teacher = new ProbabilisticCoordinateDescent()
            {
                Tolerance = 1e-10,
                Complexity = 1e+10,
            };

            var svm = teacher.Learn(x_train, y_train);

            var regression = (LogisticRegression)svm;
            return regression;
        }

        public Accord.MachineLearning.DecisionTrees.RandomForest randomforestTest(double[][] x_train, int[] y_train)
        {
            var teacher = new RandomForestLearning()
            {
                NumberOfTrees = 20,
            };
            var forest = teacher.Learn(x_train, y_train);
            return forest;
        }

        public double calcHitRatio(int[] predict, int[] output_test)
        {
            double hitRatio;
            double len = output_test.Length;
            double hit = 0;
            for(int idx = 0; idx < output_test.Length; idx++)
            {
                if (predict[idx] == output_test[idx]) hit += 1;
            }
            hitRatio = hit / len;
            return hitRatio;
        }


    }
}
