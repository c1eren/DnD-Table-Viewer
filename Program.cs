using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace dotNet1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fp_example2 = "./res/exampleData2.txt";
            string fp_example3 = "./res/exampleData3.txt";
            string fp_example4 = "./res/exampleData4.txt";
            string fp_example5 = "./res/exampleData5.txt";
            string fp_deities = "./res/deities.txt";
            string fp_spells = "./res/spells.txt";
            string fp_ironGolemLoot = "./res/ironGolemLoot.txt";

            FileReader fileReader = new FileReader();
            TableManager tableManager = new TableManager();
            tableManager.AddTable(fileReader.MakeData(fp_example2));
            tableManager.Tables[0].PrintTable();

        }
        
    }
}
