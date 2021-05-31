using System;

var s = new S(1, 2);
if (s.A is > 10) Console.WriteLine(s);
if (s.B is 30) Console.WriteLine(s);

record S(int A, int B);