#include "../headers/inputFileManager.h"
#include <iostream>
#include <fstream>
#include <sstream>

std::string InFileManager::makeData(std::string& filePath)
{
	std::string filepathStr(filePath + '\n');
	return filepathStr + readFile(filePath);
}

std::string InFileManager::readFile(std::string& filePath)
{
	std::string tString;
	std::ifstream tFile;
	tFile.exceptions(std::ifstream::failbit | std::ifstream::badbit);

	try
	{
		tFile.open(filePath);
		std::stringstream tFileStream;

		tFileStream << tFile.rdbuf();
		tFile.close();
		// Convert filestream to C++ string
		tString = tFileStream.str();
	}
	catch (std::ifstream::failure& e)
	{
		std::cout << "ERROR::FILE: " << filePath << "FILE_NOT_SUCCESSFULLY_READ" << e.what() << std::endl;
	}

	return tString;
}