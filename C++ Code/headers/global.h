#pragma once
#include <string>

 struct TableData {
	std::string name;
	unsigned int headerCount;
	std::string& data;
};