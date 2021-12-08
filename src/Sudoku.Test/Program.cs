using System;
using System.Text.Json;

const string jsonText = @"{
  ""prop1"": ""a"",
  ""prop2"": ""b"",
  ""prop3"": ""c"",
  ""prop4"": ""d""
}";

using var jsonDoc = JsonDocument.Parse(jsonText, new()
{
	AllowTrailingCommas = true,
	CommentHandling = JsonCommentHandling.Skip
});

var rootElement = jsonDoc.RootElement;
foreach (var (name, value) in rootElement.EnumerateObject())
{
	Console.WriteLine($"{name}: {value}");
}

Console.WriteLine(new string('-', 30));

var targetElement = rootElement.GetProperty("prop3");
Console.WriteLine(targetElement.GetString());

static partial class Program
{
	public static void Deconstruct(this JsonProperty @this, out string name, out JsonElement value)
	{
		name = @this.Name;
		value = @this.Value;
	}
}