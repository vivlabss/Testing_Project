using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            double[] raw_data = {  2095000,
2120000,
2092000,
2070000,
2068000,
2030000,
2009000,
2010000,
2010000,
2010000,
2004000,
1981000,
1986000,
1922000,
1922000,
1903000,
1911000,
1959000,
1965000,
1947000,
1933000,
1893000,
1901000,
1886000,
1879000,
1898000,
1918000,
1920000,
1920000,
1941000,
1978000,
1973000,
1968000,
1956000,
1973000,
1995000,
1995000,
1995000,
1970000,
1908000,
1903000,
1860000,
1874000,
1847000,
1848000,
1833000,
1873000,
1940000,
1914000,
1862000,
1861000,
1810000,
1778000,
1808000,
1824000,
1805000,
1802000,
1802000,
1788000,
1799000,
1798000,
1782000,
1809000,
1805000,
1812000,
1795000,
1793000,
1759000,
1777000,
1766000,
1752000,
1780000,
1790000,
1772000,
1748000,
1718000,
1727000,
1749000,
1746000,
1677000,
1677000,
1650000,
1650000,
1649000,
1640000,
1593000,
1586000,
1568000,
1558000,
1539000,
1553000,
1598000,
1649000,
1596000,
1644000,
1640000,
1627000,
1616000,
1643000,
1652000,
1639000,
1614000,
1573000,
1567000,
1597000,
1608000,
1589000,
1620000,
1625000,
1589000,
1590000,
1577000,
1557000,
1535000,
1545000,
1680000,
1706000,
1691000,
1619000,
1614000,
1598000,
1598000,
1600000,
1567000,
1569000,
1568000,
1571000,
1618000,
1592000,
1585000,
1558000,
1527000,
1527000,
1527000,
1527000,
1465000,
        }; // 향후 데이터를 입력받음
            double[] lag_raw_data = new double[raw_data.Length - 1];
            double[] outputs = new double[raw_data.Length - 1];
            double lifeTime;
            double mean_reversion_speed;

            for (int i = 0; i < raw_data.Length - 1; i++) lag_raw_data[i] = raw_data[i + 1];
            for (int i = 0; i < raw_data.Length - 1; i++) outputs[i] = raw_data[i] - lag_raw_data[i];

            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                Console.Write(lag_raw_data[i] + "\t");
            }
            Console.WriteLine();
            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                Console.Write(raw_data[i] + "\t");
            }
            Console.WriteLine();
            for (int i = 0; i < raw_data.Length - 1; i++)
            {
                Console.Write(outputs[i] + "\t");
            }
            Console.WriteLine();
            OrdinaryLeastSquares ols = new OrdinaryLeastSquares();

            SimpleLinearRegression regression = ols.Learn(lag_raw_data, outputs);
            mean_reversion_speed = regression.Slope;
            Console.WriteLine("slope : " + mean_reversion_speed);


            mean_reversion_speed = -1 * Math.Log(1 + mean_reversion_speed);
            Console.WriteLine("mean_reversion_speed : " + mean_reversion_speed);

            lifeTime = ( Math.Log(2) / mean_reversion_speed);
            Console.WriteLine("Half Life : " + lifeTime);

        }
    }

  
}
