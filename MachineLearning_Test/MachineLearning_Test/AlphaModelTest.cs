using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Linear;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.MachineLearning.DecisionTrees;

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



            // 회사이름을 바탕으로 데이터들을 처리 및 순위 결정하여 출력
            double[][] raw_train_data = new double[companyNames.Count][];
            double[][] raw_test_data = new double[companyNames.Count][];

            string[][] adfScore = new string[companyNames.Count][];
            double[] hurstScore = new double[companyNames.Count];
            double[] halfLife = new double[companyNames.Count];

            double[] knnHitRatio = new double[companyNames.Count];
            double[] logisticHitRatio = new double[companyNames.Count];
            double[] forestHitRatio = new double[companyNames.Count];

            KNearestNeighbors[] knn = new KNearestNeighbors[companyNames.Count];
            LogisticRegression[] logistic = new LogisticRegression[companyNames.Count];
            Accord.MachineLearning.DecisionTrees.RandomForest[] forest = new Accord.MachineLearning.DecisionTrees.RandomForest[companyNames.Count];

            for (int idx = 0; idx < companyNames.Count; idx++)
            {
                try
                {
                    raw_train_data[idx] = a.getDataPrepare(companyNames[idx], "'2016-01-01'", "'2016-12-31'");
                    Console.WriteLine(companyNames[idx] + " 성공!");
                }
                catch
                {
                    Console.WriteLine("회사이름 오류 !");
                }
                
            }   // raw_train data
            for (int idx = 0; idx < companyNames.Count; idx++)
            {
                try
                {
                    raw_test_data[idx] = a.getDataPrepare(companyNames[idx], "'2017-01-01'", "'2017-03-01'");
                    Console.WriteLine(companyNames[idx] + " 성공!");
                }
                catch
                {
                    Console.WriteLine("회사이름 오류 !");
                }
            }   // raw_test_data
            
            for(int idx = 0; idx < companyNames.Count; idx++)       
            {
                try
                {
                    adfScore[idx] = b.adfTest(raw_train_data[idx]);
                    Console.WriteLine("adf 계산 완료");
                }
                catch
                {
                    Console.WriteLine("회사 이름 오류");
                }

            }    // adfScore 세팅
            for (int idx = 0; idx < companyNames.Count; idx++)
            {
                try
                {
                    hurstScore[idx] = b.hurstTest(raw_train_data[idx]);
                    Console.WriteLine("hurst 계산 완료");
                }
                catch
                {
                    Console.WriteLine("회사 이름 오류");
                }

            }    // hurstScore 세팅
            for (int idx = 0; idx < companyNames.Count; idx++)
            {
                try
                {
                    halfLife[idx] = b.halflifeTest(raw_train_data[idx]);
                    Console.WriteLine("halflife 계산 완료");
                }
                catch
                {
                    Console.WriteLine("회사 이름 오류");
                }

            }    // halflife 세팅

            for (int idx = 0; idx < companyNames.Count; idx++)
            {
                try
                {
                    double[][] train_input = a.inputDataSetting(raw_train_data[idx]);
                    int[] train_output = a.outputDataSetting(raw_train_data[idx]);
                    double[][] test_input = a.inputDataSetting(raw_test_data[idx]);
                    int[] test_output = a.outputDataSetting(raw_test_data[idx]);

                    int len = test_input.Length;

                    int[] predict_knn = new int[test_input.Length];
                    int[] predict_logistic = new int[test_input.Length];
                    int[] predict_forest = new int[test_input.Length];

                    knn[idx] = c.knnTest(train_input, train_output);
                    logistic[idx] = c.logisticTest(train_input, train_output);
                    forest[idx] = c.randomforestTest(train_input, train_output);                

                    Console.WriteLine("학습성공");

                    for (int cnt = 0; cnt < len; cnt++)
                    {
                        predict_knn[cnt] = knn[idx].Compute(test_input[cnt]);
                        predict_logistic[cnt] = logistic[idx].Decide(test_input)[cnt] == true ? 1 : 0;
                        predict_forest[cnt] = forest[idx].Decide(test_input[cnt]);
                        Console.WriteLine("예측치 처리 완료");
                    }

                    knnHitRatio[idx] = c.calcHitRatio(predict_knn, test_output);
                    Console.WriteLine("knn 성공");
                    logisticHitRatio[idx] = c.calcHitRatio(predict_logistic, test_output);
                    Console.WriteLine("로지스틱 성공");
                    forestHitRatio[idx] = c.calcHitRatio(predict_forest, test_output);
                    Console.WriteLine("랜덤 포레스트 성공");

                }
                catch
                {
                    Console.WriteLine("회사이름 오류 !");
                }

            }    // 학습 진행


        }
    }
}
