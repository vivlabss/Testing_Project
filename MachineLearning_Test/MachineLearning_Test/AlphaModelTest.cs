using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearning_Test
{
    class AlphaModelTest
    {
        static void Main(string[] args)
        {
            DataSetting a = new DataSetting();
            List<string> companyNames = a.parseCodeHtml(a.downloadCode());
            double[] raw_data = a.getDataPrepare("SK이노베이션", "'2016-01-01'", "'2016-12-31'");

        }
    }
}
