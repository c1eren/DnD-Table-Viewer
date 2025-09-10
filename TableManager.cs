using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace dotNet1
{
    // TableManager is responsible for parsing the input data and separating the components before giving them to the table constructor
    internal class TableManager
    {
        // Custom delimiter
        private static readonly string delimiter = ";;";

        private readonly List<Table> tables = new List<Table>();
        public List<Table> Tables { get { return tables; } }

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
        internal class Table
        {
            private List<List<string>> _tableArray;

            private int _headerCount;
            private int _rowCount;
            private int _maxLineLen = 0;
            private string _horizontalBar;

            private static readonly int SCREEN_WIDTH = 120;

            public string Name { get; private set; }

            internal Table(string name, List<string> headers, List<List<string>> rows)
            {
                Name   = name;
                _headerCount = headers.Count;
                _rowCount    = rows.Count;

                _tableArray = new List<List<string>>(_rowCount + 1); // +1 for header

                for (int i = 0; i < _rowCount; i++)
                {
                    List<string> tempRow = new List<string>(_headerCount);
                    for (int j = 0; j < _headerCount; j++)
                    {
                        tempRow.Add(rows[i][j]);
                    }
                    _tableArray.Add(tempRow);
                }

                // Now insert headers at beginning
                _tableArray.Insert(0, headers);
                
                // This loop defines the width of the table columns
                for (int i = 0; i < _headerCount; i++)
                {
                    int size = _tableArray[0][i].Length;
                    if (size > _maxLineLen)
                        _maxLineLen = size;
                }
                _maxLineLen += 2;

                int maxAllowedLen = ((SCREEN_WIDTH - 1) / _headerCount) - 3;
                if (_maxLineLen > maxAllowedLen)
                    _maxLineLen = maxAllowedLen;

                int barLen = ((_maxLineLen + 3) * _headerCount) - 1; // " | " is 3 char, and I like the borders to share a corner, hence the -1
                string bar = new string('-', barLen);
                _horizontalBar = "  " + bar; // Double space is purely aesthetic
            }

            public void PrintTable()
            {
                Console.WriteLine("\n~ " + Name + " ~");
                PrintHeaders();
                PrintRows();
            }

            private void PrintHeaders()
            {

                Console.Write(_horizontalBar + "\n | ");

                // Iterate and print headers
                for (int i = 0; i < _headerCount; i++)
                {
                    string headerData = _tableArray[0][i];

                    int factor = (headerData.Length <= _maxLineLen) ? (_maxLineLen - headerData.Length) : 0;
                    string spaces = new string(' ', factor);
                    Console.Write(headerData + spaces + " | ");
                }

                Console.WriteLine("\n" + _horizontalBar);
            }
            private void PrintRows()
            {
                for (int i = 1; i < _rowCount + 1; ++i)
                {
                    // Make a temporary printable 2D string vector
                    List<string> rowData = _tableArray[i];
                    List<List<string>> variableRow = new List<List<string>>(_headerCount);

                    int overflowCount = 0;

                    Console.Write(" | ");

                    // Copy each string into its respective vector position, and build table cell for overflows
                    for (int j = 0; j < _headerCount; ++j)
                    {
                        variableRow.Add(new List<string>());
                        if (rowData[j].Length > _maxLineLen)
                        {
                            variableRow[j] = GetOverflowRows(_tableArray[i][j]);
                            if (variableRow[j].Count > overflowCount)
                                overflowCount = variableRow[j].Count;
                        }
                        else
                            variableRow[j].Add(rowData[j]);

                        string spaces = new string(' ', _maxLineLen - variableRow[j][0].Length);
                        Console.Write(variableRow[j][0] + spaces + " | ");
                    }
                    Console.Write('\n');

                	// For every overflow line
		            if (overflowCount != 0)
		            {
		            	PrintOverflowRow(overflowCount, variableRow);
                    }
                    Console.WriteLine(_horizontalBar);
                }
            }
            private List<string> GetOverflowRows(string text)
            {
                int textSize = text.Length;
                List<string> overflows = new List<string>(textSize / _maxLineLen + 1);

                int start = 0;
                while (start < textSize)
                {
                    // Get remaining char in string
                    int remaining = textSize - start;

                    // If the rest fits on one line push it then break loop
                    if (remaining <= _maxLineLen)
                    {
                        overflows.Add(text.Substring(start));
                        break;
                    }

                    int breakPos = 0;

                    int cutoff = Math.Min(start + _maxLineLen, textSize);
                    breakPos = text.LastIndexOfAny(new char[] { ' ', '-', ')' }, cutoff - 1);

                    if (breakPos != -1 && breakPos > start) // breakPos > start guardrails going behind current pos
                	{
                        overflows.Add(text.Substring(start, (breakPos - start) + 1));
                        start = breakPos + 1;
                        
                        while (start < textSize && text[start] == ' ')
                            ++start; // Skip empty spaces
                    }

                    else
                    {
                        overflows.Add(text.Substring(start, _maxLineLen - 1) + '-');
                        start += _maxLineLen - 1;
                    }
                }

                return overflows;
            }

            private void PrintOverflowRow(int overflowCount, List<List<string>> variableRow)
            {
                for (int i = 1; i < overflowCount; i++) // TODO: fix this 
                {
                   Console.Write(" | ");

                    for (int j = 0; j < _headerCount; j++)
                    {
                        // If we are printing empty cell
                        List<string> cellOverflow = variableRow[j];
                        string text;
                        if (i < cellOverflow.Count)
                            text = (cellOverflow[i]);
                        else
                            text = ("");

                        int factor = (text.Length < _maxLineLen) ? (_maxLineLen - text.Length) : 0;
                        string spaces = new string(' ', factor);
                        Console.Write(text + spaces + " | ");
                    }
                    Console.Write('\n');
                }
            }


        }

    }
}
