#ifndef _SOLVER_H
#define _SOLVER_H

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus
	// Solves a sudoku puzzle, and returns the number of solutions.
	__declspec(dllexport) int solve(
		const char* const _SudokuText,         // Puzzle string (Sudoku susser format).
		char*             _SolutionPtr,        // The result.
		int               _SolutionsCountLimit // The limit of solutions to calculate.
	);
#ifdef __cplusplus
}
#endif // __cplusplus
#endif // !_SOLVER_H