# SS0313
## 基本信息

**错误编号**：`SS0313`

**错误叙述**：

* **中文**：请尽量使用 `select`-`into` 语句代替 `let` 语句。
* **英文**：Please use `select-into` clause instead of `let` clause.

**级别**：编译器信息

**类型**：设计（Design）

## 描述

`select`-`into` 语句是一个比较特殊的执行语句。为了让后续的代码能够继续在 `select` 语句后串联，LINQ 语句是允许直接在 `select` 语句后追加 `into` 关键字，并跟上一个变量代表变量继续往下迭代。

虽然 LINQ 里包含 `let` 语句，但 `let` 语句的底层设计相当复杂，以至于它会导致代码生成一些很麻烦的语句。我们来看一个执行效果完全一样，但分别使用 `select`-`into` 语句和 `let` 语句两种不同的语句格式来实现的代码：

```csharp
int[] a = { 1, 2, 3, 4, 5, 6 };

var selection1 =
    from e in a
    select e + 1 into f
    where f % 2 == 0
    select f;
// Produces:
/*
var selection1 = Enumerable.Where(
	Enumerable.Select(
		a,
		static e => e + 1
	),
	static f => f % 2 == 0
);
*/

var selection2 =
    from e in a
    let f = e + 1
    where f % 2 == 0
    select f;
// Produces:
/*
var selection2 = Enumerable.Select(
	Enumerable.Where(
		Enumerable.Select(
			a,
			static e => e + 1
		),
		static f => f % 2 == 0
	),
	static f => f
);
*/
```

如代码所示，`selection1` 使用 `select`-`into` 语句，而 `selection2` 变量使用 `let` 语句。可以从生成代码的注释文字看出，`selection2` 最终会因为 `let` 语句的存在而产生两层 `Enumerable.Select` 语句的调用，导致多了一层冗余的 `Enumerable.Select` 语句。

可以看到，`selection2` 变量生成的代码里，最外层 `Enumerable.Select` 语句的第二个映射参数使用了 `f => f` 这样毫无意义的映射关系语句。
