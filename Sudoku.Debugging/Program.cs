using System;
using System.Numerics;

var span = getAllSets(17);
foreach (int bit in span)
{
	Console.WriteLine(bit);
}

static unsafe ReadOnlySpan<int> getAllSets(byte val)
{
	if (val == 0)
	{
		return ReadOnlySpan<int>.Empty;
	}

	int length = BitOperations.PopCount(val);
	int* resultSpan = stackalloc int[length];
	for (byte i = 0, p = 0; i < 8; i++, val >>= 1)
	{
		if ((val & 1) != 0)
		{
			resultSpan[p++] = i;
		}
	}

	return new(resultSpan, length); // SUDOKU017.
}