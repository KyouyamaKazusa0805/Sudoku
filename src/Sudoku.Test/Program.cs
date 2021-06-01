using System;

var c = new C();
c.Inner = c;

Console.WriteLine(c.ToString());

record C
{
	public int X { get; init; }
	public int Y { get; init; }
	public int Z { get; init; }

	public C? Inner { get; set; }
}