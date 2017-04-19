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
            Mean_Reversion b = new Mean_Reversion();
            Machine_Learning c = new Machine_Learning();
            AlphaModel d = new AlphaModel();

            List<string> companyNames = a.parseCodeHtml(a.downloadCode());

            double[] sk_raw_train_data = a.getDataPrepare("SK이노베이션", "'2016-01-01'", "'2016-12-31'");
            double[] sk_raw_test_data = a.getDataPrepare("SK이노베이션", "'2017-01-01'", "'2017-03-01'");

            int currentIdx = sk_raw_test_data.Length - 1;

            double[][] sk_train_input_data =  a.inputDataSetting(sk_raw_train_data);
            int[] sk_train_output_data = a.outputDataSetting(sk_raw_train_data);
            double[][] sk_test_input_data = a.inputDataSetting(sk_raw_test_data);
            int[] sk_test_output_data = a.outputDataSetting(sk_raw_test_data);

            string[] sk_adfScore = b.adfTest(sk_raw_train_data);
            double sk_hurstScore = b.hurstTest(sk_raw_train_data);
            double sk_halfLife = b.halflifeTest(sk_raw_train_data);

            d.determinePosition_MA(b.calcMovingAverage(sk_raw_train_data), sk_raw_train_data, currentIdx, true);

            d.determinePosition_ML(c.knnTest(sk_train_input_data, sk_train_output_data), 
                                    c.logisticTest(sk_train_input_data, sk_train_output_data), 
                                    c.randomforestTest(sk_train_input_data, sk_train_output_data),
                                    sk_test_input_data,
                                    currentIdx-1,
                                    true);
         
        }
    }
}
