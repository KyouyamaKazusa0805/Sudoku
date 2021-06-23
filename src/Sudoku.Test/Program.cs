#pragma warning disable IDE0060

using System;
using Sudoku.CodeGen;

Console.WriteLine();

partial class MyClass1
{
	[ProxyEquality]
	public bool PairEquals(MyClass1 left, MyClass1 right) => false;
}

partial class MyClass2
{
	[ProxyEquality]
	public static int PairEquals(MyClass2 left, MyClass2 right) => 42;
}

partial class MyClass3
{
	[ProxyEquality]
	public static bool PairEquals(MyClass1 left, MyClass3 right) => false;
}

partial class MyClass4
{
	[ProxyEquality]
	public static bool Equals(MyClass4 left) => false;
}

partial class MyClass5
{
	[ProxyEquality]
	public static bool Equals1(MyClass5 left, MyClass5 right) => false;

	[ProxyEquality]
	public static bool Equals2(MyClass5 left, MyClass5 right) => true;
}
