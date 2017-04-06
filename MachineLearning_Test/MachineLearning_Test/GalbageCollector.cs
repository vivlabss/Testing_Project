using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MachineLearning_Test
{
    class GalbageCollector
    {
        // 참고자료 : http://helloit.tistory.com/40
        // http://www.simpleisbest.net/post/2011/04/01/Review-NET-Garbage-Collection.aspx


        static void Main(string[] args)
        {
            object test = "TEST";
            Console.WriteLine("MaxGen : " + GC.MaxGeneration.ToString());
            Console.WriteLine("Current Object Gen : " + GC.GetGeneration(test));

            Console.WriteLine("Current GenCount : " + GC.CollectionCount(0).ToString());
            Console.WriteLine("Current GenCount : " + GC.CollectionCount(1).ToString());
            Console.WriteLine("Current GenCount : " + GC.CollectionCount(2).ToString());

            GC.Collect();
            Console.WriteLine("Current Object Gen : " + GC.GetGeneration(test));

            Console.WriteLine("Current GenCount : " + GC.CollectionCount(0).ToString());
            Console.WriteLine("Current GenCount : " + GC.CollectionCount(1).ToString());
            Console.WriteLine("Current GenCount : " + GC.CollectionCount(2).ToString());

            GC.Collect();
            Console.WriteLine("Current Object Gen : " + GC.GetGeneration(test));

            Console.WriteLine("Current GenCount : " + GC.CollectionCount(0).ToString());
            Console.WriteLine("Current GenCount : " + GC.CollectionCount(1).ToString());
            Console.WriteLine("Current GenCount : " + GC.CollectionCount(2).ToString());

            GC.Collect();
            Console.WriteLine("Current Object Gen : " + GC.GetGeneration(test));

            Console.WriteLine("Current GenCount : " + GC.CollectionCount(0).ToString());
            Console.WriteLine("Current GenCount : " + GC.CollectionCount(1).ToString());
            Console.WriteLine("Current GenCount : " + GC.CollectionCount(2).ToString());
        }

    }
}
