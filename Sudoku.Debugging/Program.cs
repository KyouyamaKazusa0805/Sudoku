using System;
using Sudoku.Data;

var cells = new Cells { 0, 10, 20, 40 };
Console.WriteLine($"The {nameof(cells)} {(cells.Count == 0 ? "is" : "is not")} empty."); // SUDOKU018.

var candidates = new Candidates { 0, 10, 20, 40 };
Console.WriteLine($"The {nameof(cells)} {(candidates.Count != 0 ? "is" : "is not")} empty."); // SUDOKU018.