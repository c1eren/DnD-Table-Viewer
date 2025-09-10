using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotNet1
{
    static class MainLoop
    {
        static FileReader fileReader = new FileReader();
        static TableManager tableManager = new TableManager();
        public static void Init()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string resDir = Path.Combine(exeDir, "res");

            if (Directory.Exists(resDir))
            {
                string[] files = Directory.GetFiles(resDir);

                foreach (string file in files)
                {
                    tableManager.AddTable(fileReader.MakeData(file));
                    //Console.WriteLine(file);
                }
            }
            else
                Console.WriteLine("FOLDER::DOES_NOT_EXIST: " + Path.GetFullPath(resDir));
        }

        public static void Start()
        {
            int pointer = 0;
            int tablesCount = tableManager.Tables.Count;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Please select a table to view: \n");
                for (int i = 0; i < tablesCount; i++)
                {
                    if (pointer == i)
                        Console.Write($"->     {tableManager.Tables[i].Name}\n");
                    else
                        Console.Write($"    {tableManager.Tables[i].Name}\n");
                }
                Console.Write("\n...Or press Escape to end program");

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.DownArrow)
                    pointer++;
                if (keyInfo.Key == ConsoleKey.UpArrow)
                    pointer--;

                if (pointer > tablesCount - 1)
                    pointer = tablesCount - 1;
                if (pointer < 0)
                    pointer = 0;

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    tableManager.Tables[pointer].PrintTable();
                    Console.Write("Press Enter to continue...");
                    Console.ReadLine();
                }

                if (keyInfo.Key == ConsoleKey.Escape)
                    break;
            }
        }
    }
}
    