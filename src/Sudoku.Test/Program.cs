using System;

object o = 3;

if (o is not object)
{
	Console.WriteLine(o);
}