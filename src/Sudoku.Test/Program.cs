using Sudoku.Data;
using Sudoku.Diagnostics.CodeAnalysis;

var grid = Grid.Parse("....6..3....8..2..2..5........29.5...76............1.......7.645...............7.");
_ = grid.ToString(A);

static partial class Program
{
	[IsRegex]
	private static readonly string A = @"";
}