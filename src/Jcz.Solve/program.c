#include "solver.h"
#include <stdio.h>
#include <stdlib.h>

int main(void) {
	const char* const source = "000040602004800091020000040901008006050000080200700405010000050460003900503020000";
	char result[82] = { '\0' };
	solve(source, result, 2);
	puts(result);
	//system("pause");
	return 0;
}