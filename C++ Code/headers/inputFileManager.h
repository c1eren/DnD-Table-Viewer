#pragma once

#include "../headers/global.h"

class InFileManager {
public:
	InFileManager() {}
	~InFileManager() {}

	std::string makeData(std::string& filePath);

private:
	std::string readFile(std::string& filePath);

};