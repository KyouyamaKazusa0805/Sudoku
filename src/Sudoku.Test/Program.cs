using System;

var s = new S(1, 2);
if (s is (a: 1, b: _))
{
	Console.WriteLine(nameof(s));
}

var person = new Person("Sunnie", 25, Gender.Male);
if (person is (_, _, Gender.Male))
{
	Console.WriteLine(nameof(person));
}

readonly struct S
{
	private readonly int _a, _b;

	public S(int a, int b) { _a = a; _b = b; }

	public void Deconstruct(out int a, out int b) { a = _a; b = _b; }
}

record Person(string Name, int Age, Gender Gender);

enum Gender { Male, Female };