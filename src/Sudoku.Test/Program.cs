#nullable enable

using System.Text;
using Sudoku.Diagnostics;
using Sudoku.Diagnostics.LanguageFeatures;

var fileCounter = new FileCounter(
	root: Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName,
	extension: "cs",
	withBinOrObjDirectory: false
);

await fileCounter.CountUpAsync();

var syntaxReplacer = new FileScopedNamespaceSyntaxReplacer();
foreach (string file in fileCounter.FileList)
{
	if (file.StartsWith(@"P:\Projects\Common\Sudoku\src\Sudoku.CodeGenerating"))
	{
		continue;
	}

	string content = await File.ReadAllTextAsync(file, Encoding.UTF8);

	if (syntaxReplacer.Replace(content) is not { } resultCode)
	{
		continue;
	}

	await File.WriteAllTextAsync(file, resultCode, Encoding.UTF8);
}
