#include "../headers/table.h"
#include <iostream>

#define SCREEN_WIDTH 120

// Size of console window 
std::string screenWidth("------------------------------------------------------------------------------------------------------------------------");// 121char


Table::Table(std::string& tName, Header tHeaders, std::vector<Row> tRows)
	: name(tName), headerCount(tHeaders.getSize()), rowCount(tRows.size())
{
	// Resize array to fit our table data
	tableArr.resize(rowCount, std::vector<std::string>(headerCount));

	// Each Row contains (headerCount * std::string) {exampleData2.txt has 21 rows of non-header data}
	for (std::size_t i = 0; i < rowCount; ++i)
	{
		// Return those headerCount * std::string data to rowData	
		const std::vector<std::string>& rowData = tRows.at(i).getData();

		// Copy each string into our table array at its row x column
		for (std::size_t j = 0; j < headerCount; ++j)
		{
			tableArr.at(i).at(j) = rowData.at(j);
			std::size_t size = tableArr.at(i).at(j).size();
		}
	}

	tableArr.insert(tableArr.begin(), tHeaders.getData());
	for (std::size_t i = 0; i < tableArr.at(0).size(); i++)
	{
		std::size_t size = tableArr.at(0).at(i).size();
		if (size > max_line_len)
			max_line_len = size;
	}
	max_line_len += 2;

	std::size_t maxAllowedLen = ((SCREEN_WIDTH - 1) / headerCount) - 3;
	if (max_line_len > maxAllowedLen)
		max_line_len = maxAllowedLen;

	horizontalBar = ("  " + std::string(((max_line_len + 3) * headerCount) - 1, '-')); // " | " is 3 char, and I like the borders to share a corner, hence the -1
	
	//std::cout << "max_line_len:				  " << max_line_len << std::endl;
	//std::cout << "horizontalBar.size():		  " << horizontalBar.size() << std::endl;
	//std::cout << "max_line_len * headerCount: " << max_line_len * headerCount << std::endl;
}

void Table::printTable()
{
	printName();
	printHeaders();
	printRows();
}

void Table::printName()
{
	std::cout << "~ " << name << " ~" << std::endl;
}

void Table::printHeaders()
{
	std::cout << horizontalBar << "\n";
	std::cout << " | ";

	// Iterate and print headers
	for (unsigned int i = 0; i < headerCount; i++)
	{
		std::string headerData = tableArr.at(0).at(i);

		std::size_t factor = (headerData.size() <= max_line_len) ? (max_line_len - headerData.size()) : 0;
		std::string spaces(factor, ' ');
		std::cout << headerData << spaces << " | ";
	}
	std::cout << "\n" << horizontalBar << std::endl;
}

void Table::printRows()
{
	for (std::size_t i = 1; i < rowCount + 1; ++i)
	{
		// Make a temporary printable 2D string vector
		const std::vector<std::string>& rowData = tableArr.at(i);
		std::vector<std::vector<std::string>> varRow;
		varRow.resize(headerCount);

		std::size_t overflowCount = 0;

		std::cout << " | ";

		// Copy each string into its respective vector position, and build table cell for overflows
		for (std::size_t j = 0; j < headerCount; ++j)
		{
			if (rowData.at(j).size() > max_line_len)
			{
				varRow.at(j) = (getOverflowRows(tableArr.at(i).at(j)));
				if (varRow.at(j).size() > overflowCount)
					overflowCount = varRow.at(j).size();
			}
			else
				varRow.at(j).push_back(rowData.at(j));

			std::string spaces(max_line_len - varRow.at(j).at(0).size(), ' ');
			std::cout << varRow.at(j).at(0) << spaces << " | ";
		}
		std::cout << "\n";

		// For every overflow line
		if (overflowCount != 0)
		{
			printOverflowRow(overflowCount, varRow);
		}
		std::cout << horizontalBar << "\n";
	}
}

/*
	Check the whole row and put into the printout vector
	if there is an overflow:
		create a new string vector for overflow
		fill each string in the vector with the max columnLen spaces at headerCount size
	then:
		take the cells text up to the cut-off charLen point and add to the initial printout vector
		overwrite the overflow vector at [original strings vector position]:
			and repeat the overflow check for THIS text
	then:
		continue checking the row
	if there is another overflow:
		repeat the process, but overwrite this strings vector position instead.
*/

std::vector<std::string> Table::getOverflowRows(std::string &text)
{
	std::vector<std::string> overflows;
	std::size_t textSize = text.size();
	overflows.reserve(textSize / max_line_len + 1);
		
	std::size_t start = 0;
	while (start < textSize)
	{
		// Get remaining char in string
		std::size_t remaining = textSize - start;

		// If the rest fits on one line push it then break loop
		if (remaining <= max_line_len)
		{
			overflows.emplace_back(text.substr(start));
			break;
		}

		std::size_t breakPos = 0;

		std::size_t cutoff = std::min(start + max_line_len, textSize);
		breakPos = text.find_last_of(" -)", cutoff - 1); // Same as rfind(), but for any char in the given string

		if (breakPos != std::string::npos && breakPos > start) // breakPos > start guardrails going behind current pos
		{
			overflows.emplace_back(text.substr(start, (breakPos - start) + 1));
			start = breakPos + 1;

			while (start < textSize && text[start] == ' ')
				++start; // Skip empty spaces
		}
		else
		{
			overflows.emplace_back(text.substr(start, max_line_len - 1) + '-');
			start += max_line_len - 1;
		}
	}

	return overflows;
}

void Table::printOverflowRow(std::size_t &overflowCount, std::vector<std::vector<std::string>> &varRow)
{
	for (std::size_t n = 1; n < overflowCount; ++n) // TODO: fix this 
	{
		std::cout << " | ";

		for (std::size_t k = 0; k < headerCount; ++k)
		{
			// If we are printing empty cell
			const auto& cellOverflow = varRow.at(k);
			std::string text;
			if (n < cellOverflow.size())
				text = (cellOverflow.at(n));
			else
				text = ("");
			
			std::size_t factor = (text.size() < max_line_len) ? (max_line_len - text.size()) : 0;
			std::string spaces(factor, ' ');
				std::cout << text << spaces << " | ";
			}
		std::cout << "\n";
	}
}