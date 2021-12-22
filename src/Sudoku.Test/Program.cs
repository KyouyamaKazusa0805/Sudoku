using Sudoku.Data;
using Sudoku.Diagnostics.CodeAnalysis;

var grid = Grid.Parse("Test");
_ = grid.ToString(A);

static partial class Program
{
	[IsRegex]
	private static readonly string A = @"";
}