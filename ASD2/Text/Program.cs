using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ASD.Text
{
    class Program
    {
        static void Main(string[] args)
        {
            string A = "alaalala";
            string B = "ala";
            foreach (int i in StringMatching.ComputeP(B))
            {
                Console.WriteLine(i);
            }
            foreach (int i in StringMatching.RK(A, B))
            {
                Console.WriteLine(i);
            }
        }
    }
}
