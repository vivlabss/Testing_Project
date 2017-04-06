using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

using Accord;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Models.Regression;
using Accord.Statistics.Models.Regression.Fitting;

using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace MachineLearning_Test
{
    static class Logistic_Dog_Cat
    {      

        static void Main(string[] args)
        {

            // 참고자료 : http://terrorjang.tistory.com/88
            // http://terrorjang.tistory.com/89

            // issues 01 : 메모리 부족 오류 발생 => 64비트 버전으로 빌드해야 한다 !!
            // issues 02 : 바이트 어레이 메모리 스트림 접근 불가 오류 => http://stackoverflow.com/questions/28172110/readtimeout-exception-with-memorystream

            string path_train = Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\train\train";
            string path_test = Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\test\test";
            Bitmap[] bitmaps = new Bitmap[25000];
            Bitmap[] bitmaps_test = new Bitmap[12500];
            byte[] temp01;
            double[][] inputs = new double[25000][];
            int[] outputs = new int[25000];
            OpenCvSharp.CPlusPlus.Size size = new OpenCvSharp.CPlusPlus.Size(500, 500);

            Processing_cat(path_train, bitmaps, size);
            Processing_dog(path_train, bitmaps, size);
            Processing_test(path_test, bitmaps_test, size);

            // 학습 데이터 바이트 배열로 변환하여 저장 
            for (int i = 0; i < bitmaps.Length; i++)
            {
                temp01 = imageToByteArray(bitmaps[i]);
                List<double> temp_1 = new List<double>();
                double[] temp_2;
                temp01.ToList<byte>().ForEach(b => temp_1.Add(Convert.ToDouble(b)));
                temp_2 = temp_1.ToArray<double>();
                inputs[i] = temp_2;
                Console.WriteLine(i + "바이트 배열 전환 성공");
            }
            // 라벨링 데이터 셋팅
            for (int i = 0; i < (outputs.Length / 2); i++)
            {
                outputs[i] = 0;
                outputs[i + 12500] = 1;
            }

            // 로지스틱 회귀분석
            var learner = new IterativeReweightedLeastSquares<LogisticRegression>()
            {
                Tolerance = 1e-10,
                Iterations = 100,
                Regularization = 0,
            };

            LogisticRegression regression = learner.Learn(inputs, outputs);

            double[] scores = regression.Probability(inputs);

        }

        private static void Processing_dog(string path_train, Bitmap[] bitmaps, OpenCvSharp.CPlusPlus.Size size)
        {

            for (int i = 0; i < 12500; i++)
            {
                Mat mat_dog = Cv2.ImRead(path_train + @"\dog." + i + ".jpg", LoadMode.Color);
                mat_dog = mat_dog.Resize(size,0,0,Interpolation.Linear);
                bitmaps[i + 12500] = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat_dog);
                Console.WriteLine(path_train + @"\dog." + i + ".jpg", LoadMode.Color);
                mat_dog.Dispose();
            }
        }

        private static void Processing_cat(string path_train, Bitmap[] bitmaps, OpenCvSharp.CPlusPlus.Size size)
        {
            for (int i = 0; i < 12500; i++)
            {
                Mat mat_cat = Cv2.ImRead(path_train + @"\cat." + i + ".jpg", LoadMode.Color);
                mat_cat = mat_cat.Resize(size, 0, 0, Interpolation.Linear);
                bitmaps[i] = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat_cat);
                Console.WriteLine(path_train + @"\cat." + i + ".jpg", LoadMode.Color);
                mat_cat.Dispose();
            }
        }

        private static void Processing_test(string path_test, Bitmap[] bitmaps, OpenCvSharp.CPlusPlus.Size size)
        {
            for (int i = 0; i < 12500; i++)
            {
                Mat mat_test = Cv2.ImRead(path_test + @"\" + (i+1) + ".jpg", LoadMode.Color);
                mat_test = mat_test.Resize(size, 0, 0, Interpolation.Linear);
                bitmaps[i] = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat_test);
                Console.WriteLine(path_test + @"\"+ (i+1) + ".jpg", LoadMode.Color);
                mat_test.Dispose();
            }
        }

        static byte[] imageToByteArray(this System.Drawing.Bitmap image)
        {
            
            using (var ms = new MemoryStream())
            {
                Bitmap copy = new Bitmap(image);
                copy.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                return ms.ToArray();
            }
        }

        // <testing moduule>
        //Mat mat_dog = Cv2.ImRead(path_train + @"\dog.1.jpg", LoadMode.Color);
        //mat_dog = mat_dog.Resize(size, 0, 0, Interpolation.Linear);
        //    Bitmap testing = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat_dog);
        //temp01 = imageToByteArray(testing);
        //List<double> temp_1 = new List<double>();
        //double[] temp_2;
        //temp01.ToList<byte>().ForEach(b => temp_1.Add(Convert.ToDouble(b)));
        //    temp_2 = temp_1.ToArray<double>();
    }
}
