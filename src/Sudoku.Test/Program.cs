using System;
using Sudoku.Data;

var grid = SudokuGrid.Parse("080070003000600000009003800050760090608001007900080320400007000003090000010000002");
Console.WriteLine(grid);
Console.WriteLine(grid.EigenString);
Console.WriteLine(grid.Token);

var newGrid = new SudokuGrid(grid.Token);
Console.WriteLine(newGrid.ToString("0"));