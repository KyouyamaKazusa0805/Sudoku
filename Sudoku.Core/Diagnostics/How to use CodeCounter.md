# How to use `CodeCounter`

标题：**如何使用 `CodeCounter`**

So easy! Write code like this:

很简单！这样写代码就好：

```c#
//Line counter.
string solutionFolder = Solution.PathRoot;
var codeCounter = new CodeCounter(solutionFolder, @".*\.cs$");
int linesCount = codeCounter.CountCodeLines(out int filesCount);
Console.WriteLine($"Found {filesCount} files, {linesCount} lines.");
```



You can get the number of code lines in all csharp files!

你就可以得到所有 C# 文件的代码行数了。