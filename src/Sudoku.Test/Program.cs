using System;

var zhangSan = new Person("Zhang san", 25, Gender.Male, new("Li si", 50, Gender.Male));

if (
	zhangSan is
	{
		Name: "Zhang San",
		Age: 24,
		Father: { Father: { Name: "Zhang 'er" } },
		Mother: { Name: "Li si", Gender: Gender.Female }
	}
)
{
	Console.WriteLine("Zhang san does satisfy that condition.");
}

record Person(string Name, int Age, Gender Gender, Person? Father = null, Person? Mother = null);
enum Gender { Male, Female }