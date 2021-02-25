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
SudokuGrid grid3 = default; // SUDOKU021.
var grid4 = default(SudokuGrid); // SUDOKU021.
var candidates5 = default(Candidates); // SUDOKU021.
Candidates candidates6 = default; // SUDOKU021.