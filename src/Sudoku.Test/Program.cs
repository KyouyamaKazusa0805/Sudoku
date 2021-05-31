using System;

var s = new S(1, 2);
if (s is { A: not > 3 }) Console.WriteLine(s);
if (s.A is not < 3) Console.WriteLine(s);

record S(int A, int B);