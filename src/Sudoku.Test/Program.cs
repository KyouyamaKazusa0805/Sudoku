using Sudoku.Data;
using Sudoku.Diagnostics.CodeAnalysis;

var grid = Grid.Parse("Test");
_ = grid.ToString(A);

static partial class Program
{
	public static int Prop
	{
		get => 10;
		set { }
	}


	[IsRegex]
	private static readonly string A = @"";
}