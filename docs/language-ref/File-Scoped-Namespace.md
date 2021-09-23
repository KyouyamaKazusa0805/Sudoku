# 基于文件的命名空间指令

C# 10 有一个减少缩进的神奇语法：单行命名空间声明。这个实际上是模仿了 Java 的 `package` 的基本语法，为了减少缩进就这么干的。

按照道理来说，C# 确实应该很早就有这个特性，因为 GitHub 上 C# 语法特性讨论区里经常有人想要让 C# 团队减少这样的无意义的缩进。但是 C# 设计团队考虑了一些兼容性问题因此迟迟没有搞这个。

## 语法

写法其实和以前的写法如出一辙，不过少了一层大括号。

```csharp
// Old style.
namespace System.Collections
{
    internal static class HashHelper
    {
        // ..
    }
}
```

现在我们省去第 3 行和第 8 行的大括号，而对 `namespace` 后追加分号：

```csharp
// New style.
namespace System.Collections;

internal static class HashHelper
{
    // ..
}
```

这样确实要清爽不少。

## 简化版写法不支持嵌套

稍微要注意的地方是，因为在早期的 C# 里，命名空间声明由于有大括号，所以支持嵌套：

```csharp
namespace System
{
    namespace Collections
    {
        // ...
    }

    namespace Diagnostics
    {
        // ...
    }
}
```

不过在简化语法后，这样的嵌套不再支持。如果你非得这么写代码，只能使用原始语法（像上面这样）。

所以，C# 10 里规定，如果你使用简化版的命名空间声明语句，一个文件只能有一个 `namespace` 的声明，并且必须要放在最开头（仅次于 `#define` 这些预处理指令和 `using` 指令之后）的位置上。比如下面这样就是不允许的：

```csharp
namespace A;

// ...

namespace B; // Wrong.

// ...
```

一个文件一旦用了新语法后，就不能使用超过一个的命名空间声明的简化写法了。

## `namespace` 和 `using` 指令的先后顺序

接着是 `namespace` 和 `using` 指令的混用的写法。

我们一般的顺序是先 `using` 指令，然后是 `namespace` 指令，然后才是内容。因为 `namespace` 出现了简化版，所以它的长相更加接近于 `using` 指令（甚至从语法上，`namespace` 指令和 `using` 指令现在只有关键字不同）。所以刚开始用会很不习惯。一定要注意先 `using` 指令后才是 `namespace` 指令。如果反过来的话，C# 早期语法的规范，这种写法是兼容的，但我们一般不那么写。

举个例子。

```csharp
namespace System;

using Custom;

internal class Test
{
    // ...
}
```

这个代码是把 `using` 指令放在了 `namespace` 的下方。按规则来说我们一般是反过来，所以这样写的话不算错，但不符合一般的习惯写法，因为它等价于早期写法：

```csharp
namespace System
{
    using Custom;

    internal class Test
    {
        // ...
    }
}
```

即直接把 `using` 放在了 `namespace` 和这对大括号的里面。所以一定注意习惯上的书写顺序。