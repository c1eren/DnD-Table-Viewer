#include "../headers/tableManager.h"
#include <iostream>

// Custom delimiter
std::string delimiter = ";;";

void TableManager::addTable(std::string& data)
{
	tables.emplace_back(makeTable(data));
	currentTableFilepath = "";
	currentTableName = "";
}

Table TableManager::makeTable(std::string& data)
{

	std::vector<std::string> lines(parseData(data));
	std::size_t index = 1;

	currentTableFilepath = lines.front();
	lines.erase(lines.begin());
	std::string name = lines.front();
	currentTableName = name;

	Header headers(makeHeaders(lines, index));
	
	checkData(index < lines.size(), 1);
	checkData(!(headers.getSize() == 0), 2);
	std::vector<std::string> rowData(makeRowData(lines, index));
	checkData((rowData.size() % headers.getSize() == 0), 3); 
	std::vector<Row> rows(buildRows(rowData, headers.getSize()));

	return { name, headers, rows };
}

std::vector<std::string> TableManager::parseData(std::string& data)
{
	std::size_t end	  = 0;
	std::size_t start = 0;
	std::vector<std::string> lines;

	// Convert data to string vector
	while ((end = data.find('\n', start)) != std::string::npos)
	{
		lines.push_back(data.substr(start, end - start));
		start = end + 1;
	}
	if (start < data.size())
		lines.push_back(data.substr(start));

	return lines;
}

Header TableManager::makeHeaders(std::vector<std::string>& lines, std::size_t& index)
{
	// Create headers
	std::vector<std::string> headers;

	while (index < lines.size() && lines.at(index) != delimiter)
	{
		headers.push_back(lines.at(index));
		if (index < lines.size())
			++index;
	}
	return headers;
}

std::vector<std::string> TableManager::makeRowData(std::vector<std::string>& lines, std::size_t& index)
{
	// Get data to make rows
	std::vector<std::string> rowData;

	if (index < lines.size())
		++index;

	rowData.reserve(lines.size() - index);

	for (; index < lines.size(); ++index)
	{
		//if (!lines.at(index).empty() || lines.at(index) == "")
			rowData.emplace_back(lines.at(index));
	}
	return rowData;
}

std::vector<Row> TableManager::buildRows(std::vector<std::string>& rowData, std::size_t headerSize)
{
	// Build rows
	std::vector<Row> rows;

	// For each row
	for (unsigned int i = 0; i < rowData.size(); i += headerSize)
	{
		std::vector<std::string> temp;
		temp.reserve(headerSize);

		// For each cell for headerCount
		for (unsigned int j = 0; j < headerSize; ++j)
		{
			temp.emplace_back(rowData.at(i + j));
		}
		rows.emplace_back(temp); // Have a look at this later
	}
	return rows;
}

void TableManager::checkData(bool expression, unsigned int type)
{
	switch (type) {
	case 1:
		if (!expression) // index >= lines.size()
		{
			std::cerr << "IN::TABLE::\"" << currentTableName << "\"::AT::FILEPATH::\"" << currentTableFilepath << "\"\n";
			std::cerr << "NO '" + delimiter + "' FOUND TO SEPARATE HEADERS AND DATA";
			throw std::runtime_error("NO '" + delimiter + "' FOUND TO SEPARATE HEADERS AND DATA");
		}
		break;

	case 2:
		if (!expression) // headers.size() == 0
		{
			std::cerr << "IN::TABLE::\"" << currentTableName << "\"::AT::FILEPATH::\"" << currentTableFilepath << "\"\n";
			std::cerr << "NO HEADERS FOUND" << std::endl;
			throw std::runtime_error("NO HEADERS FOUND");
		}
		break;

	case 3:
		if (!expression) // rowData.size() % headers.size() == 0
		{
			std::cerr << "IN::TABLE::\"" << currentTableName << "\"::AT::FILEPATH::\"" << currentTableFilepath << "\"\n";
			std::cerr << "DATA LINES ARE NOT DIVISIBLE BY THE HEADER COUNT" << std::endl;
			throw std::runtime_error("DATA LINES ARE NOT DIVISIBLE BY THE HEADER COUNT");
		}
		break;

	default:
		if (!expression)
		{
			std::cerr << "IN::TABLE::\"" << currentTableName << "\"::AT::FILEPATH::\"" << currentTableFilepath << "\"\n";
			std::cerr << "CHECK ERROR LOG" << std::endl;
			throw std::runtime_error("UNKNOWN ERROR");
		}
		break;
	}
}