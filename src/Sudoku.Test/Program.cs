using System;
using Sudoku.Text;

const string jsonText = @"{
  ""prop1"": ""a"",
  ""prop2"": ""b"",
  ""prop3"": ""c"",
  ""prop4"": ""d""
}";

g();

static void g()
{
	var doc = new ResourceDocument(jsonText);
	foreach (var (name, value) in doc)
	{
		Console.WriteLine($"{name}: {value}");
	}

	Console.WriteLine(new string('-', 30));

	string targetValue = doc["prop3"];
	Console.WriteLine(targetValue);
}
