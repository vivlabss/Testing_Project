using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace MachineLearning_Test
{
    class Hurst
    {
        static void Main(string[] args)
        {
            // 참고자료 : http://www.financialwisdomforum.org/gummy-stuff/hurst.htm
            // H = 0.5 이면 GBM, H = 0 이면 평균회귀, H = 1 이면 추세성향


            double[] raw_data = SqlCommandPrepare().ToArray(); // 향후 데이터를 입력받음
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
            for (int idx = 0; idx < raw_data.Length; idx++)  temp_sums += (raw_data[idx] - mean_raw_data) * (raw_data[idx] - mean_raw_data);
            temp_sums /= raw_data.Length;
            std = Math.Sqrt(temp_sums);

            hurst = Math.Log(R / std) / Math.Log(raw_data.Length);
            Console.WriteLine(hurst);
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
