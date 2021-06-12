using System;

object o = 100;

switch (o)
{
	case int q:
		Console.WriteLine(q);
		break;
	case var _:
		Console.WriteLine(o);
		break;
	default:
	{ }
}

Console.WriteLine(o switch
{
	int q => q,
	var _ => o
});