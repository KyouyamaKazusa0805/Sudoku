using System;

var s = new S();
Console.WriteLine(s.ToString());

struct S
{
	public int A { get; }
	public readonly int B { get; }
	public int C { readonly get; set; }
	public int D { get; set; }
}