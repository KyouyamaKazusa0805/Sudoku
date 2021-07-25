using System;
using System.Collections.Generic;

var dic = new ValueDictionary<Gender, int>();
dic.Add(Gender.Female, 10);
dic.Add(Gender.Male, 20);

foreach (var (key, value) in dic)
{
	Console.WriteLine($"{key.ToString()}: {value.ToString()}");
}
Console.WriteLine();
foreach (var key in dic.Keys)
{
	Console.WriteLine(key);
}
Console.WriteLine();
foreach (int value in dic.Values)
{
	Console.WriteLine(value);
}


enum Gender { Male, Female };