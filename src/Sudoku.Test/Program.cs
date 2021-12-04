using System;
using System.IO;
using System.Text.Json;

string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
string targetPath = $@"{desktop}\test.json";
await using var stream = File.OpenRead(targetPath);
var result = await JsonSerializer.DeserializeAsync<JsonElement>(stream);

foreach (var i in result.EnumerateObject())
{
	Console.WriteLine(i.ToString());
}