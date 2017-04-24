using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;

namespace DataHandling
{
    class xmlParsing
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(170, 20);
            dataRead();
        }
        static void dataRead()
        {
            string path1 = @"C:\Users\acorn\Desktop\DB\DS\";
            int itemLength1 = Directory.GetFiles(path1).Length;

            string path2 = @"C:\Users\acorn\Desktop\DB\PT\";
            int itemLength2 = Directory.GetFiles(path2).Length;

            string path3 = @"C:\Users\acorn\Desktop\DB\TM\";
            int itemLength3 = Directory.GetFiles(path3).Length;

            string path4 = @"C:\Users\acorn\Desktop\DB\UT\";
            int itemLength4 = Directory.GetFiles(path4).Length;

            XmlReader xmlReader;
            XmlReaderSettings setting = new XmlReaderSettings();
            setting.IgnoreComments = true;
            setting.IgnoreWhitespace = true;

            StreamWriter sw = new StreamWriter(@"C:\Users\acorn\Desktop\DB\DS\DS.txt");

            for(int idx = 0; idx < itemLength1; idx++)
            {
                xmlReader = XmlReader.Create(path1 + "db (" + (idx + 1) + ").xml", setting);

                while (xmlReader.Read())
                {
                    sw.WriteLine(xmlReader.Value);
                }
                sw.WriteLine();
            }

            sw.Close();
            sw.Dispose();


            sw = new StreamWriter(@"C:\Users\acorn\Desktop\DB\PT\PT.txt");

            for (int idx = 0; idx < itemLength2; idx++)
            {
                xmlReader = XmlReader.Create(path2 + "db (" + (idx + 1) + ").xml", setting);

                while (xmlReader.Read())
                {
                    sw.WriteLine(xmlReader.Value);
                }
                sw.WriteLine();
            }

            sw.Close();
            sw.Dispose();

            sw = new StreamWriter(@"C:\Users\acorn\Desktop\DB\TM\TM.txt");

            for (int idx = 0; idx < itemLength3; idx++)
            {
                xmlReader = XmlReader.Create(path3 + "db (" + (idx + 1) + ").xml", setting);

                while (xmlReader.Read())
                {
                    sw.WriteLine(xmlReader.Value);
                }
                sw.WriteLine();
            }

            sw.Close();
            sw.Dispose();

            sw = new StreamWriter(@"C:\Users\acorn\Desktop\DB\UT\UT.txt");

            for (int idx = 0; idx < itemLength4; idx++)
            {
                xmlReader = XmlReader.Create(path4 + "db (" + (idx + 1) + ").xml", setting);

                while (xmlReader.Read())
                {
                    sw.WriteLine(xmlReader.Value);
                }
                sw.WriteLine();
            }

            sw.Close();
            sw.Dispose();

        }
    }
}
