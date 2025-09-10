#pragma once

#include <string>
#include <vector>

class Header {
public:
	Header(std::vector<std::string> headerData) : data(headerData) {};

	std::vector<std::string> getData() { return data; }
	std::size_t getSize() { return data.size(); }

private:
	std::vector<std::string> data;
};