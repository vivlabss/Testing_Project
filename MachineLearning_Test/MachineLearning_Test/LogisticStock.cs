using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

using Accord;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Models.Regression;

namespace MachineLearning_Test
{
    class LogisticStock
    {
        static void Main(string[] args)
        {
            List<double> set = trainPrepare();
            List<double> tSet = testPrepare();
            double[] raw_data = set.ToArray();
            double[] raw_test = tSet.ToArray();
            Console.WriteLine(raw_data.Length);

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

            Console.WriteLine();
            
            for (int i = 0; i < testSet.Length; i++) Console.WriteLine(regression.Decide(testSet[i]));

        }

        static List<double> trainPrepare()
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
                        "SELECT PRICE_CLOSE FROM SK이노베이션 WHERE DATE BETWEEN '2016-10-01' AND '2016-12-31' ORDER BY DATE ASC";

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

        static List<double> testPrepare()
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
                        "SELECT PRICE_CLOSE FROM SK이노베이션 WHERE DATE BETWEEN '2017-01-01' AND '2017-03-01' ORDER BY DATE ASC";

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
