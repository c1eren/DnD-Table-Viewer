using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace dotNet1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MainLoop.Init();
            MainLoop.Start();

            Console.WriteLine("\n\nGoodbye...");
        }
        
    }
}
