using System;
using Sudoku.Diagnostics.CodeAnalysis;

Console.WriteLine("Hello, world!");

class A<[Self] T> where T : A<T>
{
}

class B<T>
{
}

class C<[Self] T> where T : notnull, C<T>
{
}

class D<[Self] T, U> where T : D<T, U> where U : class
{
}