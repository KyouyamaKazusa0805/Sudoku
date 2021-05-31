using System;

var s = new S(1, 2);
if (s is not null and { A: 10 }) Console.WriteLine(s);
if (s is { B: 10 } and not null) Console.WriteLine(s);
if (s is null or { A: 100 }) Console.WriteLine();
if (s is { B: 100 } or null) Console.WriteLine();
if (s is { A: 100 } or null or not null and { B: 200 }) Console.WriteLine();

record S(int A, int B);