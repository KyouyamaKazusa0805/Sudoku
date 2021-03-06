很简单！这样写代码就好：

```csharp
using System;
using System.IO;
using Sudoku.Diagnostics;
using static System.Console;

string root = Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName;

WriteLine(new FileCounter(root, "cs", withBinOrObjDirectory: false).CountUp());
```

你就可以得到所有 C# 文件的代码行数了。