using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;

using Accord;
using Accord.MachineLearning;
using Accord.Statistics;

using OpenCvSharp;

namespace MachineLearning_Test
{
    static class Logistic_Dog_Cat
    {      

        static void Main(string[] args)
        {
            ConvertToBitmap();
        }

        public static void Labeling()
        {

        }

        public static void ConvertToBitmap()
        {
            // http://blog.naver.com/PostView.nhn?blogId=choihszg&logNo=120158078851 참고자료

            string path_train = Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\train\train";
            string path_test = Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\test\test";
            DirectoryInfo di_train = new DirectoryInfo(path_train);
            DirectoryInfo di_test = new DirectoryInfo(path_test);
            Bitmap[] bitmaps = new Bitmap[di_train.GetFiles().Length];

            for (int i = 0; i < di_train.GetFiles().Length / 2; i++)
            {
                using (IplImage src_cat = new IplImage(Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\train\train\cat." + i + ".jpg"))
                using (IplImage dst_cat = new IplImage(src_cat.Size, BitDepth.U8, 1))
                {
                    src_cat.Smooth(src_cat, SmoothType.Gaussian, 5);
                    src_cat.Threshold(dst_cat, 0, 255, ThresholdType.Otsu);

                    bitmaps[i] = dst_cat.ToBitmap();
                }         
            }
            for (int i = 0; i < di_train.GetFiles().Length / 2; i++)
            {
                using (IplImage src_dog = new IplImage(Directory.GetCurrentDirectory() + @"\Dog_Cat_Data\train\train\dog." + i + ".jpg"))
                using (IplImage dst_dog = new IplImage(src_dog.Size, BitDepth.U8, 1))
                {
                    src_dog.Smooth(src_dog, SmoothType.Gaussian, 5);
                    src_dog.Threshold(dst_dog, 0, 255, ThresholdType.Otsu);

                    bitmaps[i + di_train.GetFiles().Length / 2] = dst_dog.ToBitmap();
                }
            }
            Console.WriteLine(bitmaps.Length);

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
