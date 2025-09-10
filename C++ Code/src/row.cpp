#include "../headers/row.h"

std::size_t Row::getSize()
{
	std::size_t temp = 0;
	for (unsigned int i = 0; i < data.size(); ++i)
	{
		temp += data.at(i).size();
	}
	return temp;
}