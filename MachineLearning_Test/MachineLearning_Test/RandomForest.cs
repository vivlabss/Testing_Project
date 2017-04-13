using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Accord;
using Accord.Math;
using Accord.MachineLearning.DecisionTrees.Learning;
using Accord.MachineLearning.DecisionTrees;

namespace MachineLearning_Test
{
    class RandomForest
    {
        static void Main(string[] args)
        {
            List<double> set = SqlCommandPrepare();
            double[] raw_data = set.ToArray();
            Console.WriteLine(raw_data.Length);

            double[][] dataSet = new double[raw_data.Length][];
            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                double temp = raw_data[i];
                dataSet[i] = new double[] { temp };
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
                NumberOfTrees = 10,
            };
            var forest = teacher.Learn(dataSet, outputs);
            int[] predicted = forest.Decide(dataSet);
            for(int i = 0; i < predicted.Length; i++) Console.WriteLine(predicted[i]);
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
