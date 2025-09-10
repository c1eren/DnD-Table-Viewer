#pragma once

#include <vector>
#include "../headers/header.h"
#include "../headers/row.h"
#include "../headers/global.h"

class Table {
public:
	Table(std::string& tName, Header tHeaders, std::vector<Row> tRows);
	void printTable();

private:
	// Outer vector is rows, inner vector is columns (cells in that row)
	std::vector< std::vector<std::string> > tableArr;

	std::size_t headerCount	 = 0;
	std::size_t rowCount	 = 0;
	std::size_t max_line_len = 0;

	std::string name;
	std::string horizontalBar;

private:
	void printName();
	void printHeaders();
	void printRows();

	std::vector<std::string> getOverflowRows(std::string& text);
	void printOverflowRow(std::size_t& overflowCount, std::vector<std::vector<std::string>>& varRow);
};