using System;

P p = new();
string q = p!.ToString();

P a = new();
int r = a!.A;

P b = new();
int s = b![3];

Console.WriteLine(q);
Console.WriteLine(r);
Console.WriteLine(s);

class P
{
	public int A => 3;

	public int this[int index] => index + 6;

	public override string ToString() => string.Empty;
}