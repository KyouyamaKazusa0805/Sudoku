namespace Sudoku.MinLex;

internal unsafe struct Mapper
{
	public fixed byte Cell[81];

	public fixed byte Label[10];
}
