using System;
using System.IO;
using Sudoku.Diagnostics.LanguageFeatures;

string targetPath = Path.Combine(
	Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
	"Test.cs"
);

ISyntaxReplacer f = new FileScopedNamespaceSyntaxReplacer();
string content = await File.ReadAllTextAsync(targetPath);
string? processedResult = await f.ReplaceAsync(content);

Console.WriteLine(processedResult);