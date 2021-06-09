using System;

int i = 30;

if (i is 20)
{
	Console.WriteLine(i);
}
if (i is not 100)
{
	Console.WriteLine(i);
}