# 该项目额外的彩蛋：注入式代码分析器

这个项目还带有一定程度的代码分析器，它可以在你编译代码的时候开始运作，产生编译器错误或编译器警告信息。对于一些 API 的使用会比较有帮助。

这个代码分析器采用的是[源代码生成器](https://docs.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview)的机制来完成的，源代码生成器的特性是 C# 9 开始支持的特性，如果想要了解用法，请参考给出的这个链接内容。另外，这个代码分析器会对 API 的用法用途，以及基本的 C# 语法配合这个解决方案的内容进行分析和处理。

所有跟这个项目有关的代码分析器的编号均区分开官方 .NET 分析编号（CS 系列、CA 系列等）——这个项目使用 SCA 系列的编号。SCA 是 Solution-wide Code Analysis（全解决方案范围的代码分析）的首字母缩写。包含的分析编号有如下的一些。如果你在编译的时候遇到了这些问题，可查看它们来调整和完善对 API 的使用和代码的书写。

* 源代码生成器相关

  * SCA0001（提供的特性参数必须至少有一个）
  * SCA0002（提供的特性参数不可为空）
  * SCA0003（提供的 SCA 系列编号必须以 SCA 字母开头）
  * SCA0004（该特性只用于非嵌套的类型）
  * SCA0005（该特性只用于非抽象类型）
  * SCA0006（代码分析器类型必须是非泛型的）
  * SCA0007（代码分析器类型必须是分部类型）
  * SCA0008（代码分析器类型应为密封的类型）
  * SCA0009（代码分析器类型必须实现 `ISyntaxContextReceiver` 接口）
  * SCA0010（代码分析器类型里自带了 `_cancellationToken` 字段，但源代码生成器会自动生成该字段）
  * SCA0011（代码分析器类型里自带了 `Diagnostics` 属性，但源代码生成器会自动生成该属性）
  * SCA0012（代码分析器类型的命名应以 `SyntaxChecker` 结尾）
  * SCA0013（源代码生成器会对所有 `ref struct` 类型生成一些代码，因此需要有 `partial` 修饰符）

* 特性修饰相关

  * SCA0101（标识 `SelfAttribute` 的泛型参数，缺乏奇异递归模板模式的泛型参数约束）
  * SCA0102（标识 `SelfAttribute` 的泛型参数，虽然有泛型约束，但仍缺乏奇异递归模板模式的泛型参数约束）
  * SCA0107（标识 `RestrictAttribute` 的参数必须是指针类型）
  * SCA0108（该方法标识 `RestrictAttribute` 的参数数量必须至少为两个）
  * SCA0109（标识了 `RestrictAttribute` 的参数不可传入相同的指针数值）
  * SCA0110（标识了 `AnonymousInnerTypeAttribute` 的类型不可用于实例化为临时变量）
  * SCA0111（标识了 `AnonymousInnerTypeAttribute` 的类型必须包含一个编译器生成的无参构造器）
  * SCA0112（标识了 `AnonymousInnerTypeAttribute` 的类型最好是引用类型，否则会导致装箱行为）
  * SCA0113（不要暴露标识了 `AnonymousInnerTypeAttribute` 的类型）
  * SCA0114（标识了 `AnonymousInnerTypeAttribute` 的类型应为密封的）
  * SCA0115（标识了 `AnonymousInnerTypeAttribute` 的类型必须实现一定的接口才能体现它的用途）
  * SCA0116（标识了 `AnonymousInnerTypeAttribute` 的类型不可用于传参、返回值等其它非临时变量的地方上）
  * SCA0119（标识了 `IsDiscardAttribute` 的参数不能被显式使用，除非用的是 `nameof` 表达式）
  * SCA0120（标识了 `IsDiscardAttribute` 的参数不能有 `out`、`ref`、`params` 修饰符）
  * SCA0121（标识了 `IsDiscardAttribute` 的参数已经是弃元，特性标记没有意义）
  * SCA0122（特性 `InitializationOnlyAttribute` 只能用于字段或自动属性）
  * SCA0123（标识了 `InitializationOnlyAttribute` 的字段不要暴露给外界使用）
  * SCA0124（标识了 `InitializationOnlyAttribute` 的属性的 setter 不要暴露给外界使用）
  * SCA0125（无法给标识了 `InitializationOnlyAttribute` 的成员重新赋值）
  * SCA0126（标识了 `StepSearcherAttribute` 的特性必须实现 `IStepSearcher` 接口）
  * SCA0127（已经实现了 `IStepSearcher` 接口的类型应该标识 `StepSearcherAttribute` 特性）
  * SCA0128（标识了 `StepSearcherAttribute` 的类型，除非 `IsDirect` 属性为 `true`，否则必须优先调用 `FastProperties.Initialize` 静态方法才可使用）
  * SCA0129（标识了 `StepSearcherAttribute` 且属性 `PuzzleNotRelying` 为 `true` 的话，那么参数 `grid` 应为弃元，不可使用）
  * SCA0130（标识了 `StepSearcherAttribute` 的类型不可是抽象类型）
  * SCA0131（标识了 `StepSearcherAttribute` 的类型必须带有无参构造器）
  * SCA0132（标识了 `StepSearcherAttribute` 的类型不可是静态类型）

* 命名和使用规范相关

  * SCA0301（本地函数应以驼峰命名法命名）
  * SCA0302（扩展方法的第一个参数必须命名为 `@this`）
  * SCA0303（该属性是自带 `readonly` 修饰的语义的，所以无需显式使用该修饰符）
  * SCA0304（该属性位于值类型中，且仅用于返回，因此该属性是只读的，所以请标记 `readonly` 修饰符到该属性上）
  * SCA0305（请勿暴露具体类型的函数指针类型的成员）

  * SCA0402（`GetPinnableReference` 方法需要标记 `[EditorBrowsable(EditorBrowsableState.Never)]` 防止显式使用）
  * SCA0403（`GetPinnableReference` 方法不应为静态的）
  * SCA0404（`GetPinnableReference` 方法不能返回 `void` 类型）
  * SCA0405（`GetPinnableReference` 非无参的时候无需标记 `[EditorBrowsable(EditorBrowsableState.Never)]`）
  * SCA0406（`GetPinnableReference` 方法必须返回 `ref` 或 `ref readonly` 修饰后的类型）
  * SCA0407（无法为标识了 `CallerArgumentExpressionAttribute` 的参数显式赋值）
  * SCA0408（标识了 `CallerArgumentExpressionAttribute` 参数的类型必须是 `string?`）
  * SCA0409（无法给 `__arglist` 参数修饰 `CallerArgumentExpressionAttribute`）
  * SCA0410（传入给标识了 `CallerArgumentExpressionAttribute` 的参数必须是一个实际存在的成员名称）
  * SCA0411（解构函数 `Deconstruct` 必须返回 `void`）
  * SCA0412（解构函数 `Deconstruct` 必须的所有参数都必须修饰 `out`；不修饰或修饰别的关键字均是不允许的）
  * SCA0413（解构函数 `Deconstruct` 的参数都不可使用 `IsDiscardAttribute` 特性）
  * SCA0414（避免无参解构函数）
  * SCA0415（避免只有一个参数的解构函数）
  * SCA0416（解构函数需要标记 `[EditorBrowsable(EditorBrowsableState.Never)]` 防止显式调用）
  * SCA0417（除非解构函数是扩展方法，否则不能是泛型的）

* API 使用相关

  * SCA0501（请使用 `Undefined` 字段）
  * SCA0502（请使用 `IsUndefined` 属性）
  * SCA0503（请使用 `Grid.GetEnumerator` 简化 `Grid.EnumerateCandidates` 调用）
  * SCA0504（请使用 `==` 和 `!=` 运算符代替静态 `Grid.Equals` 方法）
  * SCA0505（不要给 `Parse` 或 `TryParse` 方法的第一个参数传入 `null` 值）
  * SCA0506（请使用 `Empty` 字段）
  * SCA0507（请使用 `IsEmpty` 属性）
  * SCA0509（该字符串不是合法的格式化字符串）
  * SCA0510（该参数的实际使用比特位数超出了该数据类型设计时候定义的比特位数）
  * SCA0511（不建议使用 `stackalloc` 表达式实例化对象）
  * SCA0512（请使用 `Empty` 字段）
  * SCA0514（请使用 `CoveredLine` 属性）
  * SCA0515（参数 `index` 必须在 0 到 80 之间）
  * SCA0516（参数规定的坐标不合法）
  * SCA0517（请使用 `==` 或 `!=` 运算符代替 `Equals` 实例方法）
  * SCA0518（不要给 `Parse` 或 `TryParse` 方法的第一个参数传入 `null` 值）
  * SCA0519（请使用 `-` 运算符）
  * SCA0520（集合初始化器仅用于无参构造器或传入 `int[]` 数组参数的构造器）