using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

using FANNCSharp.Double;

namespace MachineLearning_Test
{
    static class Logistic_Dog_Cat_Accord
    {
        static void Main(string[] args)
        {
            // 참고자료 : http://terrorjang.tistory.com/88
            // http://terrorjang.tistory.com/89

            // issues 01 : 메모리 부족 오류 발생 => 64비트 버전으로 빌드해야 한다 !!
            // issues 02 : 바이트 어레이 메모리 스트림 접근 불가 오류 => http://stackoverflow.com/questions/28172110/readtimeout-exception-with-memorystream
            // trial : 64빌드 타겟팅 빌드 + configuration <gcAllowVeryLargeObjects enabled="true" />

            string path_train = Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\train\train";
            string path_test = Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\test\test";
            Bitmap[] bitmaps = new Bitmap[250]; // 25000
            Bitmap[] bitmaps_test = new Bitmap[125]; // 12500
            byte[] temp01;
            double[] temp_2;
            List<double> temp_1 = new List<double>();
            double[][] inputs = new double[250][]; // 25000
            double[][] tests = new double[125][]; // 12500
            double[][] outputs = new double[250][]; // 25000
            OpenCvSharp.CPlusPlus.Size size = new OpenCvSharp.CPlusPlus.Size(32, 32); // 결국 사이즈를 타협했다 ㅠㅠ

            Processing_cat(path_train, bitmaps, size);
            Processing_dog(path_train, bitmaps, size);
            Processing_test(path_test, bitmaps_test, size);

            // 학습 데이터 바이트 배열로 변환하여 저장 
            for (int i = 0; i < bitmaps.Length; i++)
            {
                temp01 = imageToByteArray(bitmaps[i]);
                bitmaps[i] = null;
                temp01.ToList<byte>().ForEach(b => temp_1.Add(Convert.ToDouble(b)));
                temp_2 = temp_1.ToArray<double>();
                inputs[i] = temp_2;
                Console.WriteLine(temp_2.Length);
                temp_1.Clear();
                Console.WriteLine(i + "바이트 배열 전환 성공");
            }

            // 테스트 데이터 바이트 배열로 변환하여 저장
            for (int i = 0; i < bitmaps_test.Length; i++)
            {
                temp01 = imageToByteArray(bitmaps_test[i]);
                bitmaps_test[i] = null;
                temp01.ToList<byte>().ForEach(b => temp_1.Add(Convert.ToDouble(b)));
                temp_2 = temp_1.ToArray<double>();
                tests[i] = temp_2;
                temp_1.Clear();
                Console.WriteLine(i + "바이트 배열 전환 성공");
            }

            // 라벨링 데이터 셋팅
            for (int i = 0; i < (outputs.Length / 2); i++)
            {
                outputs[i] = new double[] { 0} ;
                outputs[i + (outputs.Length / 2)] = new double[] { 1 };
            }

            Console.WriteLine("라벨링 완료");
        
            NeuralNet network = new NeuralNet(3, 2, 4150,1);
            network.LearningRate = 2;
            network.SetActivationFunction(FANNCSharp.ActivationFunction.SIGMOID, 1, 2);
            network.SetActivationFunction(FANNCSharp.ActivationFunction.SIGMOID, 2, 4150);
            network.SetActivationFunction(FANNCSharp.ActivationFunction.SIGMOID, 3, 2);
            TrainingData training = new TrainingData();
            training.SetTrainData(inputs, outputs);
            network.TrainOnData(training, 10000, 1, (float)0.05);


            float epoch = 1;

            while (true)
            {
                epoch = network.TrainEpoch(training);
                Console.WriteLine(epoch);
                if (epoch < 0.05) break;
            }


            StreamWriter sw = new StreamWriter("data_result.csv", false, Encoding.UTF8);
            sw.WriteLine("id,label");          

            for (int i = 0; i < tests.Length; i++)
            {
                double[] result = network.Run(tests[i]);
                Console.WriteLine("actual=" +  result[i]);
                sw.WriteLine((i + 1) + "," + result[i]);
            }
            sw.Close();

            Console.WriteLine("완료");
        }

        private static void Processing_dog(string path_train, Bitmap[] bitmaps, OpenCvSharp.CPlusPlus.Size size)
        {

            for (int i = 0; i < (bitmaps.Length / 2); i++)
            {
                Mat mat_dog = Cv2.ImRead(path_train + @"\dog." + i + ".jpg", LoadMode.GrayScale);
                mat_dog = mat_dog.Resize(size, 0, 0, Interpolation.Linear);
                bitmaps[i + (bitmaps.Length / 2)] = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat_dog);
                Console.WriteLine(path_train + @"\dog." + i + ".jpg");
                mat_dog.Dispose();
                mat_dog = null;
            }
        }

        private static void Processing_cat(string path_train, Bitmap[] bitmaps, OpenCvSharp.CPlusPlus.Size size)
        {
            for (int i = 0; i < bitmaps.Length / 2; i++)
            {
                Mat mat_cat = Cv2.ImRead(path_train + @"\cat." + i + ".jpg", LoadMode.GrayScale);
                mat_cat = mat_cat.Resize(size, 0, 0, Interpolation.Linear);
                bitmaps[i] = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat_cat);
                Console.WriteLine(path_train + @"\cat." + i + ".jpg");
                mat_cat.Dispose();
                mat_cat = null;
            }
        }

        private static void Processing_test(string path_test, Bitmap[] bitmaps, OpenCvSharp.CPlusPlus.Size size)
        {
            for (int i = 0; i < bitmaps.Length; i++)
            {
                Mat mat_test = Cv2.ImRead(path_test + @"\" + (i + 1) + ".jpg", LoadMode.GrayScale);
                mat_test = mat_test.Resize(size, 0, 0, Interpolation.Linear);
                bitmaps[i] = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat_test);
                Console.WriteLine(path_test + @"\" + (i + 1) + ".jpg");
                mat_test.Dispose();
                mat_test = null;

            }
        }

        static byte[] imageToByteArray(this System.Drawing.Bitmap image)
        {
            using (var ms = new MemoryStream())
            {
                Bitmap copy = new Bitmap(image);
                copy.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                copy.Dispose();
                copy = null;
                return ms.ToArray();
            }

        }
    }
}