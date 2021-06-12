using System;

int[] p = { 3, 8, 1, 6, 5, 4, 7, 2, 9 };
if (p.Length == 5) Console.WriteLine(p[0]);
if (p.Length != 5) Console.WriteLine(p[0]);
if (p is { Length: 5 }) Console.WriteLine(p[0]);
if (p is { Length: not 5 }) Console.WriteLine(p[0]);
if (p is not { Length: 5 }) Console.WriteLine(p[0]);
if (p is not { Length: not 5 }) Console.WriteLine(p[0]);