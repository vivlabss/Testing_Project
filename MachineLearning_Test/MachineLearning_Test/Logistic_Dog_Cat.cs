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

            // 메모리 부족 오류 발생

            string path_train = Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\train\train";
            string path_test = Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\test\test";
            DirectoryInfo di_train = new DirectoryInfo(path_train);
            DirectoryInfo di_test = new DirectoryInfo(path_test);
            Bitmap[] bitmaps = new Bitmap[di_train.GetFiles().Length];
 
            Processing_cat(path_train, di_train, bitmaps);

            Processing_dog(path_train, di_train, bitmaps);

            //Console.WriteLine(bitmaps.Length);
        }

        private static void Processing_dog(string path_train, DirectoryInfo di_train, Bitmap[] bitmaps)
        {
            OpenCvSharp.CPlusPlus.Size size = new OpenCvSharp.CPlusPlus.Size(500, 500);
            for (int i = 0; i < di_train.GetFiles().Length / 2; i++)
            {
                Mat mat_dog = Cv2.ImRead(path_train + @"\dog." + i + ".jpg", LoadMode.Color);
                mat_dog = mat_dog.Resize(size,0,0,Interpolation.Linear);
                bitmaps[i + di_train.GetFiles().Length / 2] = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat_dog);
                Console.WriteLine(path_train + @"\dog." + i + ".jpg", LoadMode.Color);
            }
        }

        private static void Processing_cat(string path_train, DirectoryInfo di_train, Bitmap[] bitmaps)
        {
            OpenCvSharp.CPlusPlus.Size size = new OpenCvSharp.CPlusPlus.Size(500, 500);
            for (int i = 0; i < di_train.GetFiles().Length / 2; i++)
            {
                Mat mat_cat = Cv2.ImRead(path_train + @"\cat." + i + ".jpg", LoadMode.Color);
                mat_cat = mat_cat.Resize(size, 0, 0, Interpolation.Linear);
                bitmaps[i] = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(mat_cat);
                Console.WriteLine(path_train + @"\cat." + i + ".jpg", LoadMode.Color);
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
