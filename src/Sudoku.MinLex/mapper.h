#ifndef MAPPER_H
#define MAPPER_H

#include <string.h>

typedef struct {
	//the map is composed for transformation of the canonicalized sub-grid to the original one, so that
	//originalGrid[i] = label[canonicalGrid[cell[i]]]
	char cell[81];
	char label[10];
} Mapper;

#endif // !MAPPER_H
