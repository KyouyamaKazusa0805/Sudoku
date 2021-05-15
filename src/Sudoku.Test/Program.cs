using System;
using System.Diagnostics.CodeAnalysis;

for (var f = TestEnum.A; f < TestEnum.C; f++)
{
	Console.WriteLine(f);
}

[Closed]
enum TestEnum { A, B, C }