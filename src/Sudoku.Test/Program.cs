using System;

var r = new R(1);
var s = new S();
Console.WriteLine(r.ToString());
Console.WriteLine(s.ToString());

record R(int A);
record S
{
	public S InnerMember { get; init; } = new();
}