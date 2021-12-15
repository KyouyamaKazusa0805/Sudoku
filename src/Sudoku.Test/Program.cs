using System;
using System.Globalization;
using Sudoku.Data.Resources;

const string jsonText = @"{
  ""prop1"": ""a"",
  ""prop2"": ""b"",
  ""prop3"": ""c"",
  ""prop4"": ""d""
}";

var doc = new ResourceDocument(CultureInfo.CurrentCulture, jsonText);
foreach (var (name, value) in doc)
{
	Console.WriteLine($"{name}: {value}");
}

Console.WriteLine(new string('-', 30));

string targetValue = doc["prop3"];
Console.WriteLine(targetValue);