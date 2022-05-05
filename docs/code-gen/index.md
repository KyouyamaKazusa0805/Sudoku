# 源代码生成器简介

该项目使用 C# 9 和 C# 10 提供的源代码生成器的 API 生成一些模板代码，为了简化书写的代码内容，也为了保持代码文件的整洁。C# 一共提供了两种生成源代码生成器的接口类型：

* `ISourceGenerator`：基本的接口类型，提供基本的源代码生成过程；
* `IIncrementalGenerator`：提供高性能环境下的源代码生成。

本项目会使用到它们来完成源代码的生成过程。

该项目的源代码生成器包含如下的一些。请查阅下面的文字以及链接了解更多。

## 源代码生成器列表

本部分列举一下项目所使用到的源代码生成器的基本类型，以及生成的具体内容。

* [解构函数生成器](auto-decon)：生成解构函数的代码。
* [实现 `CompareTo` 方法的生成器](auto-impl-comparable)：生成关于 `IComparable<>` 接口实现的代码。
* [实现 `GetEnumerator` 方法的生成器](auto-impl-enumerable)：生成关于 `IEnumerable<>` 接口实现的代码。
* [自动重载大小比较运算符的生成器](auto-overloads-comparison-op)：生成关于 `IComparisonOperators<,,>` 接口实现的运算符重载代码。
* [自动重载相等性比较运算符的生成器](auto-overloads-equality-op)：生成关于 `IEqualityOperators<,,>` 接口实现的运算符重载代码。
* [自动重写 `Equals` 方法的生成器](auto-overrides-equals)：生成关于基类型 `Equals` 虚方法或非密封方法的重写代码，同时也会实现 `IEquatable<>` 接口。
* [自动重写 `GetHashCode` 方法的生成器](auto-overrides-get-hash-code)：生成关于基类型 `GetHashCode` 虚方法或非密封方法的重写代码。
* [自动重写 `ToString` 方法的生成器](auto-overrides-to-string)：生成关于基类型 `ToString` 虚方法或非密封方法的重写代码。

**`GlobalConfigValueGenerator`**（实现 `IIncrementalGenerator`）

只给源代码生成器提供服务。用于给源代码生成器的项目提供基本的程序信息，比如版本号以及项目名称。

**`RefStructOverridensGenerator`**（实现 `ISourceGenerator`）

为 `ref struct` 类型提供从 `ValueType` 派生下来的方法的默认实现。由于 `ref struct` 类型不可以任何方式装箱，因此实现它们（尤其是 `Equals(object?)` 是没有任何调用意义的，因此避免用户误用，源代码生成器将防止用户手动调用它们。

**`DisableParameterlessConstructorGenerator`**（实现 `ISourceGenerator`）

禁止用户使用无参构造器的源代码生成器。该源代码生成器将对值类型的无参构造器进行默认生成，以及特性修饰，防止用户手动定义它们。

**`StepSearcherOptionsGenerator`**（实现 `ISourceGenerator`）

对实现了 `IStepSearcher` 接口的类型生成默认的 `Options` 属性信息的相关代码。

**`BitOperationsGenerator`**（实现 `ISourceGenerator`）

对 `BitOperations` 静态类型生成额外的方法，用于扩展比特位的相关处理操作。

## 使用到的额外 API

本部分列举一下前面一节说明到的源代码生成器所依赖的 API。

**[`Microsoft.Diagnostics.CodeAnalysis` 命名空间](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis)**

这是源代码生成器依赖的不可缺少的 API 存储命名空间。Nuget 包下载地址在[这里](https://www.nuget.org/packages/Microsoft.CodeAnalysis)。

**`DisableParameterlessConstructorAttribute`**

该特性类型用于表示一个结构不建议也不允许使用它的无参构造器。

* 属性 `SuggestedMemberName`：表示推荐用户代替无参构造器使用的成员名称。
* 属性 `Message`：表示错误信息，告知用户为什么这么用无参构造器是错误的用法。

**`SearcherConfigurationAttribute<TStepSearcher> where TStepSearcher : class, IStepSearcher`**

该特性提供基本的生成选项。

* 属性 `Priority`：表示该技巧搜索器的优先级数值。数值越小越会被优先调用；不建议设置为负数。
* 属性 `DisplayingLevel`：表示该技巧搜索器所处的显示级别。在全盘技巧搜索的时候，为了考虑到性能的优化，如果设置了显示级别，那么比该级别低的技巧会被显示出来，而超过该级别的技巧不会被显示。
* 属性 `EnabledAreas`：表示该技巧可以用在什么模块上。
* 属性 `DisabledReason`：如果 `EnaabledAreas` 属性为默认数值，该属性则表示为什么要禁用此技巧搜索器。
