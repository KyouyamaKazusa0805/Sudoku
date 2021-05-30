using System;

var s = new S?(new(1, 2));
if (s.HasValue && s.Value is var t) Console.WriteLine(t);
if (s == null) Console.WriteLine();
if (s != null) Console.WriteLine(s);
if (s is null) Console.WriteLine();
if (s is not null) Console.WriteLine(s);

var o = new R(1, 2D, 3F, "4");
if (o != null) Console.WriteLine(o);
if (o == null) Console.WriteLine();

readonly struct S { public S(int a, int b) { A = a; B = b; } public int A { get; } public int B { get; } }
record R(int A, double B, float C, string D);