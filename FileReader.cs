using System;
using System.IO;
using System.Text;

namespace dotNet1
{
    public class FileReader
    {
        public string MakeData(string filePath)
        {
            // Append filePath and newline, then contents
            return filePath + Environment.NewLine + ReadFile(filePath);
        }

        private string ReadFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath, Encoding.UTF8);
            }
            catch (IOException e)
            {
                Console.WriteLine($"ERROR::FILE: {filePath} FILE_NOT_SUCCESSFULLY_READ {e.Message}"); // Proper logging framework not necessary currently
                return string.Empty;
            }
        }

    }
}
