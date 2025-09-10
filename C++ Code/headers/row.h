#pragma once

#include <string>
#include <vector>

class Row {
public:
	Row(std::vector<std::string>& rowData) : data(rowData) {}
	~Row() {}

	std::vector<std::string> getData() { return data; }
	std::size_t getSize();

private:
	std::vector<std::string> data;
};