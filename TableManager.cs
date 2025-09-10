using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace dotNet1
{
    // TableManager is responsible for parsing the input data and separating the components before giving them to the table constructor
    internal class TableManager
    {
        // Custom delimiter
        private static readonly string delimiter = ";;";

        private readonly List<Table> tables = new List<Table>();
        private string currentTableName = "";
        private string currentTableFilePath = "";

        public void AddTable(string data)
        {
            tables.Add(MakeTable(data));
            currentTableName     = "";
            currentTableFilePath = "";
        }

        private Table MakeTable(string data)
        {
            var envNewLine = Environment.NewLine;

            List<string> lines = new List<string>(
                data.Split(new string[] { envNewLine }, StringSplitOptions.None)
                );

            currentTableFilePath = lines[0];
            currentTableName     = lines[1];

            //Headers 
            int delimiterIndex = lines.IndexOf(delimiter);
            if (delimiterIndex == -1)
                throw new ArgumentException("DELIMITER::NOT_FOUND_IN_DATA"); // Needs testing

            List<string> headers = lines.GetRange(2, delimiterIndex - 2);
            int headerCount = headers.Count;

            // Rows
            delimiterIndex++;
            lines = lines.GetRange(delimiterIndex, lines.Count - delimiterIndex);
            if (lines.Count % headerCount != 0)
                throw new ArgumentException("DATA LINES::NOT_DIVISIBLE_BY_HEADER_COUNT");

            List<List<string>> rows = new List<List<string>>(lines.Count / headerCount); // Outer List capacity

            // For each row 
            for (int i = 0; i < lines.Count; i += headerCount)
            {
                List<string> row = lines.GetRange(i, headerCount);
                rows.Add(row); // Keep an eye on this
            }

            return new Table(currentTableName, headers, rows);
        }

        // Table is responsible for printing rows and handling overflows
        private class Table
        {
            public static readonly int SCREEN_WIDTH = 120;

            List<List<string>> tableArray;

            private readonly int headerCount;
            private readonly int rowCount;
            private readonly int maxLineLen = 0;

            private readonly string name;
            private readonly string horizontalBar;

            public Table(string name, List<string> headers, List<List<string>> rows)
            {
                this.name   = name;
                headerCount = headers.Count;
                rowCount    = rows.Count;

                tableArray = new List<List<string>>(rowCount + 1); // +1 for header

                for (int i = 0; i < rowCount; i++)
                {
                    List<string> tempRow = new List<string>(headerCount);
                    for (int j = 0; j < headerCount; j++)
                    {
                        tempRow.Add(rows[i][j]);
                    }
                    tableArray.Add(tempRow);
                }

                // Now insert headers at beginning
                tableArray.Insert(0, headers);
                
                // This loop defines the width of the table columns
                for (int i = 0; i < headerCount; i++)
                {
                    int size = tableArray[0][i].Length;
                    if (size > maxLineLen)
                        maxLineLen = size;
                }
                maxLineLen += 2;

                int maxAllowedLen = ((SCREEN_WIDTH - 1) / headerCount) - 3;
                if (maxLineLen > maxAllowedLen)
                    maxLineLen = maxAllowedLen;

                int barLen = ((maxLineLen + 3) * headerCount) - 1; // " | " is 3 char, and I like the borders to share a corner, hence the -1
                string bar = new string('-', barLen);
                horizontalBar = "  " + bar; // Double space is purely aesthetic
            }
        }

    }
}
