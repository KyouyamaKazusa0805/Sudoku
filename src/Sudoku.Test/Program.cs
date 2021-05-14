using System;
using Sudoku.Data;
using Sudoku.Resources;

var grid = new SudokuGrid();
Console.WriteLine(grid);
SudokuGrid.ValueChanged(ref grid, new());

Console.WriteLine(TextResources.Current.Hello);
Console.WriteLine(TextResources.Current.KeyToGet);