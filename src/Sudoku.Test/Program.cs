using System;

Console.WriteLine();

[Flags]
enum TestEnum
{
	A = 1,
	B = 2,
	C = A | B,
	D,
	E = 14
}