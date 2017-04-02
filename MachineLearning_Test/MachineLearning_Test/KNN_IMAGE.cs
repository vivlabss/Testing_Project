using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Net;

using Accord;
using Accord.MachineLearning;

namespace MachineLearning_Test
{
    static class knn_image
    {
        static void Main(string[] args)
        {
            byte[] temp01;
            byte[] test;
            double[] temp02;
            double[][] inputs = new double[40][];
            int[] outputs = new int[40];

            for (int i = 0; i < 20; i++)
            {
                Image image = Image.FromFile(@"C:\Users\youli\Desktop\프로그래밍\C#\Testing_Project\MachineLearning_Test\MachineLearning_Test\Data\train\false" + (i + 1) + ".bmp");
                temp01 = imageToByteArray(image);
                List<double> temp_1 = new List<double>();
                double[] temp_2;
                temp01.ToList<byte>().ForEach(b => temp_1.Add(Convert.ToDouble(b)));
                temp_2 = temp_1.ToArray<double>();
                inputs[i] = temp_2;
            }

            for (int i = 0; i < 20; i++)
            {
                Image image = Image.FromFile(@"C:\Users\youli\Desktop\프로그래밍\C#\Testing_Project\MachineLearning_Test\MachineLearning_Test\Data\train\true" + (i + 1) + ".bmp");
                temp01 = imageToByteArray(image);
                List<double> temp_1 = new List<double>();
                double[] temp_2;
                temp01.ToList<byte>().ForEach(b => temp_1.Add(Convert.ToDouble(b)));
                temp_2 = temp_1.ToArray<double>();
                inputs[i + 20] = temp_2;
            }

            for (int i = 0; i < 20; i++)
            {
                outputs[i] = 0;
                outputs[i + 20] = 1;
            }
            //C:\Users\com\Desktop\Programming\images\train￦10.jpg
            KNearestNeighbors knn = new KNearestNeighbors(k: 3, classes: 2, inputs: inputs, outputs: outputs);

            for (int i = 0; i < 10; i++)
            {
                Image tester = Image.FromFile(@"C:\Users\youli\Desktop\프로그래밍\C#\Testing_Project\MachineLearning_Test\MachineLearning_Test\Data\test\" + (i + 1) + ".bmp");
                temp01 = imageToByteArray(tester);
                List<double> temp_11 = new List<double>();
                double[] temp_22;
                temp01.ToList<byte>().ForEach(b => temp_11.Add(Convert.ToDouble(b)));
                temp_22 = temp_11.ToArray<double>();
                //Console.WriteLine(knn.Compute(temp_22));
            }
        }

        static byte[] imageToByteArray(this System.Drawing.Image image)
        {
            using (var ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                return ms.ToArray();
            }
        }
    }
}
