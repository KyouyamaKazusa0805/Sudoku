using System;
using System.Diagnostics.CodeAnalysis;

Console.WriteLine($"12341");

for (var f = TestEnum.A; f < TestEnum.C; f++)
{
	Console.WriteLine(f);
}

[Closed]
enum TestEnum { A, B, C }