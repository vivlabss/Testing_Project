using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Accord;
using Accord.MachineLearning;
using Accord.Statistics.Models.Regression.Linear;

using System.Data;
using System.Data.SqlClient;

namespace MachineLearning_Test
{
    class ADF_Test
    {
        // 참고자료 : http://www.statisticshowto.com/adf-augmented-dickey-fuller-test/
        // http://higheredbcs.wiley.com/legacy/college/hill/0471723606/excel/using_excel_for_principles_of_econometrics3e.pdf
        // http://reliawiki.org/index.php/Simple_Linear_Regression_Analysis
        // http://www.stat.yale.edu/Courses/1997-98/101/linmult.htm
        // https://www3.nd.edu/~rwilliam/stats2/l02.pdf
        // http://dept.stat.lsa.umich.edu/~kshedden/Courses/Stat401/Notes/401-multreg.pdf
        // Multiple Linear Regression Testing - http://hspm.sph.sc.edu/Courses/J716/pdf/716-3%20Multiple%20Regression.pdf

        static void Main(string[] args)
        {


            double[] raw_data = SqlCommandPrepare().ToArray();
            double[] diff_raw_data = new double[raw_data.Length - 1];
            double[] lag_raw_data = new double[raw_data.Length - 1]; 
            double[] diff_lag_raw_data = new double[raw_data.Length - 2];

            double[] output_diff = new double[raw_data.Length - 2];
            double[] input_lag = new double[raw_data.Length - 2];
            double[] input_diff_lag = new double[raw_data.Length - 2];
            double[] predicted = new double[raw_data.Length - 2];
            double[][] inputs = new double[raw_data.Length - 2][];
            string critical_values = "error";

            // diff
            for (int i = 0; i < raw_data.Length-1; i++) diff_raw_data[i] = raw_data[i + 1] - raw_data[i];
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
            for (int i = 0; i < raw_data.Length - 2; i++) inputs[i] = new double[] { input_lag[i] , input_diff_lag[i]};
            MultipleLinearRegression regression = olss.Learn(inputs, output_diff);
            for (int i = 0; i < raw_data.Length - 2; i++) predicted[i] = regression.Transform(inputs[i]);
            Console.WriteLine(regression.Weights[0] + "\t" + regression.Weights[1]);

            // 음의 t-value보다 작거나 양의 t-value보다 크다면 정상시계열, 그렇지 않다면 랜덤과정이다.
            if ((raw_data.Length - 2) <=25)
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
            Console.WriteLine(Calc_T(inputs, output_diff, predicted, regression)[0] + "\t" + Calc_T(inputs, output_diff, predicted, regression)[1]
                + "\n" + critical_values);

        }

        static double[] Calc_T(double[][] input, double[] output , double[] predict, MultipleLinearRegression regression)
        {
            double std1 = 0 , std2 = 0, error=0;
            double xMean1 = 0, xMean2 = 0;
            double temp1 = 0 , temp2 = 0;
            double[] tScore = new double[2];

            for (int i = 0; i < output.Length; i++) error += (output[i] - regression.Transform(input)[i]) * (output[i] - regression.Transform(input)[i]);
            error /= output.Length -3;
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
            std1 =error / Math.Sqrt(std1);

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

        static List<double> SqlCommandPrepare()
        {
            string connectionString = @"Data Source =.\SQLEXPRESS; Initial Catalog = Stock_Data; Integrated Security = True; Connect Timeout = 15; Encrypt = False; TrustServerCertificate = True; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
            List<double> priceSet = new List<double>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(null, connection);

                    command.CommandText =
                        "SELECT PRICE_CLOSE FROM SK이노베이션 ORDER BY DATE ASC";

                    command.ExecuteNonQuery();

                    SqlDataReader dataReader = command.ExecuteReader();
                    while (dataReader.Read())
                    {
                        priceSet.Add(Convert.ToDouble(dataReader["PRICE_CLOSE"].ToString()));
                    }

                }
                catch
                {
                    throw;
                }
            }

            return priceSet;
        }
    }
}
