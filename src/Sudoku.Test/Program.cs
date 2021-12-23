using System;
using Sudoku.Data;

// Full:       ....6..3....8..2..2..5........29.5...76............1.......7.645...............7.
// Simplified: *6..3.,*8..2..,2..5*,*29.5..,.76*,*1..,*7.64,5*,*7.
var grid = Grid.Parse("*6..3.,*8..2..,2..5*,*29.5..,.76*,*1..,*7.64,5*,*7.");
Console.WriteLine(grid.ToString(".*"));

static partial class Program
{
}