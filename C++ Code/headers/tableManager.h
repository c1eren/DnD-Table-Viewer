#pragma once

#include <vector>
#include "../headers/table.h"
#include "../headers/global.h"

class TableManager {
public:
	TableManager() {}
	~TableManager(){}

	void addTable(std::string& data);
	std::vector<Table> getTables() const { return tables; }

private:
	std::vector<Table> tables;
	std::string currentTableFilepath;
	std::string currentTableName;

private:
	Table makeTable(std::string& data);
	std::vector<std::string> parseData(std::string& data);
	Header makeHeaders(std::vector<std::string>& lines, std::size_t& index);
	std::vector<std::string> makeRowData(std::vector<std::string>& lines, std::size_t& index);
	std::vector<Row> buildRows(std::vector<std::string>& rowData, std::size_t headerSize);
	void checkData(bool expression, unsigned int type);
};