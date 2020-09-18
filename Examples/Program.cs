/*
 * Here I listed some examples for basic and elementary using for APIs.
 * If you want to use the solution, you can take them to your projects.
 * 
 * Here I use the global-style namespaces. The reason why so is that
 * I can tell you the details of the classes/structs.
 * 
 * Sunnie
 */

using System;

// Example 1:
// Using grid parser and formatter to get a Grid instance.
#if PARSE_GRID_USING_GRID || false
{
	var grid = Sudoku.Data.Grid.Parse("000400000000007028800026490000300809090874050107009000068740005450600000000003000");
	Console.WriteLine(grid.ToString());
	Console.WriteLine(grid.ToString("."));
	Console.WriteLine(grid.ToString("0"));
}
#endif

// Example 2:
// Using grid parser and formatter to get a ValueGrid instance.
#if PARSE_GRID_USING_VALUEGRID || false
{
	var grid = Sudoku.Data.ValueGrid.Parse("58090000100008570000000100047+30+9012000010400+7+156000+4890004000000+496300+10200009074:255 577 578 579 192 597 897");
	Console.WriteLine(grid.ToString());
	Console.WriteLine(grid.ToString("."));
	Console.WriteLine(grid.ToString("#"));
}
#endif

// Example 3:
// Solve a puzzle using brute forces.
#if SOLVE_PUZZLE_BY_BRUTE_FORCES || false
{
	var grid = Sudoku.Data.Grid.Parse("580900001000085700000001000470000120000104000056000089000400000009630000200009074");
	var solver = new Sudoku.Solving.BruteForces.Bitwise.UnsafeBitwiseSolver();
	var analysisResult = solver.Solve(grid);

	Console.WriteLine(analysisResult);
}
#endif

// Example 4:
// Solve a puzzle using human techniques.
#if SOLVE_PUZZLE_BY_LOGICAL_TECHNIQUES || true
{
	string text = "580900001000085700000001000470000120000104000056000089000400000009630000200009074";
	if (new Sudoku.Solving.BruteForces.Bitwise.UnsafeBitwiseSolver().CheckValidity(text, out string? result))
	{
		var grid = Sudoku.Data.Grid.Parse(text);
		var analysisResult = new Sudoku.Solving.Manual.ManualSolver().Solve(grid);

		Console.WriteLine(grid);
	}
}
#endif