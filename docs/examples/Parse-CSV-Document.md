# 解析 CSV 文档

在代码分析器的项目里面包含有 CSV 文件用于存储每一个错误信息的相关数据，这个时候可能需要书写类来完成内容的解析，这个时候我们直接使用 `CsvDocument` 类型即可。这个类型位于 `System.IO.Csv` 命名空间下。这个类型是实现了 `IDisposable` 接口的，意味着我会强烈建议你使用 `using` 来完成手动释放。

```csharp
// Initializes an instance of type 'CsvDocument'.
using var reader = new CsvDocument(
	path: Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
		"Test.csv"
	),
	withHeader: true,
	delimiter: ','
);

// Iterate on each field line.
await foreach (string[]? fields in reader)
{
	if (fields is null)
	{
		continue;
	}

    // Use the value.
	Console.WriteLine(string.Join(" | ", fields));
}
```

这个类型实现了同步和异步两种迭代器类型。同步的话，使用 `foreach` 循环即可；如果是异步迭代器的话，请使用 `await foreach` 完成迭代。