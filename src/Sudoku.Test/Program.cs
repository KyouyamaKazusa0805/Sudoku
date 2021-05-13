using System;
using System.Numerics;
using Sudoku.Data;

var cells = new Cells();

SudokuGrid grid = default;
Console.WriteLine(grid.ToString("#"));
Console.WriteLine(cells.Count != 0);

Console.WriteLine(GetAllSets(3).ToString());

static unsafe ReadOnlySpan<int> GetAllSets(byte val)
{
	if (val == 0)
	{
		return ReadOnlySpan<int>.Empty;
	}

	int length = BitOperations.PopCount(val);
	int* ptrArrResult = stackalloc int[length];
	for (byte i = 0, p = 0; i < 8; i++, val >>= 1)
	{
		if ((val & 1) != 0)
		{
			ptrArrResult[p++] = i;
		}
	}

	return new(ptrArrResult, length);
}