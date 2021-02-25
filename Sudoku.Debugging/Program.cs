#pragma warning disable 219

using Sudoku.Data;

var grid = new SudokuGrid(); // SUDOKU021.
var cells = new Cells(); // SUDOKU021.
var cells2 = new Cells { 3, 10, 40 };
var candidates = new Candidates(); // SUDOKU021.
var candidates2 = new Candidates { 3, 10, 40 };
SudokuGrid grid2 = new(); // SUDOKU021.
Cells cells3 = new(); // SUDOKU021.
Candidates candidates3 = new(); // SUDOKU021.
Candidates candidates4 = new() { 1, 10 };