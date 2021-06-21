#pragma warning disable IDE0003
#pragma warning disable IDE0032
#pragma warning disable IDE0044

using System;

Console.WriteLine();

struct MyStruct
{
	private int _a;

	public int A => this._a;
}