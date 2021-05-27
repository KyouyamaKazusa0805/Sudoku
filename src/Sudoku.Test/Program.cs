using System;

P? a = new();
P? b = null;
string? x = a?.ToString();
string? y = b?.ToString();
string z = a!.ToString();
Console.WriteLine(x);
Console.WriteLine(y);
Console.WriteLine(z);

class P
{
	public override string ToString() => string.Empty;
}