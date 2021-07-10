using System;
using System.IO;
using System.IO.Csv;

using var reader = new CsvDocument(
	path: Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
		"Test.csv"
	),
	withHeader: true,
	delimiter: ","
);

await foreach (string[]? fields in reader.ReadToEndAsync())
{
	if (fields is null)
	{
		continue;
	}

	Console.WriteLine(string.Join(" | ", fields));
}