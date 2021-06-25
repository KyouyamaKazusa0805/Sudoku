using System;
using System.Linq;

int[] s = { 1, 3, 45, 6, 8 };
var selection = from e in s select e;

Console.WriteLine(selection);