using System;
using Sudoku.Data;

var grid = SudokuGrid.Parse("400050060010400007008000300100040700003000005060870000600000070009010400070003001");

Console.WriteLine(grid.ToString());