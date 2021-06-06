using System;
using Sudoku.CodeGen;

Console.WriteLine();

[AutoDeconstruct("_a", "_b", "_c")]
partial class Temp
{
	private readonly int _a, _b, _c;


	public Temp(int a, int b, int c) => (_a, _b, _c) = (a, b, c);
}