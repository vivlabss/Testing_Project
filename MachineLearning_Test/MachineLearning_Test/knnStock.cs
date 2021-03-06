﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Accord.MachineLearning;
using Accord.Statistics;

namespace MachineLearning_Test
{
    class knnStock
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

            KNearestNeighbors knn = new KNearestNeighbors(k: 3, classes: 2, inputs:dataSet, outputs:outputs);

            for (int i = 0; i < testSet.Length; i++) Console.WriteLine(knn.Compute(testSet[i]));
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
                    SqlCommand command = new SqlCommand("getStockData", connection);
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    SqlParameter param = new SqlParameter("@table_id", System.Data.SqlDbType.VarChar, 200);
                    param.Value = "거북선1호";
                    command.Parameters.Add(param);
                    param = new SqlParameter("@dateStart", System.Data.SqlDbType.VarChar, 50);
                    param.Value = "'2016-01-01'";
                    command.Parameters.Add(param);
                    param = new SqlParameter("@dateEnd", System.Data.SqlDbType.VarChar, 50);
                    param.Value = "'2016-12-31'";
                    command.Parameters.Add(param);

                    command.ExecuteNonQuery();
                    SqlDataReader dataReader = command.ExecuteReader();

                    while (dataReader.Read())
                    {
                        priceSet.Add(Convert.ToDouble(dataReader["PRICE_CLOSE"].ToString()));
                    }
                    connection.Close();
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
                connection.Close();
            }

            return priceSet;
        }
    }
}
