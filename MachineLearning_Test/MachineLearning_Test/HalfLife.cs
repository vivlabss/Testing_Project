using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using Accord;
using Accord.MachineLearning;
using Accord.Statistics.Models.Regression.Linear;

namespace MachineLearning_Test
{
    class HalfLife
    {
        // 참고자료 : http://marcoagd.usuarios.rdc.puc-rio.br/revers.html
        static void Main(string[] args)
        {
            double[] raw_data = SqlCommandPrepare().ToArray(); // 향후 데이터를 입력받음
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

            lifeTime = ( Math.Log(2) / mean_reversion_speed);
            Console.WriteLine("Half Life : " + lifeTime);

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
