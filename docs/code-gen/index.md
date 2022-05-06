# 源代码生成器简介

## 介绍

该项目使用 C# 9 和 C# 10 提供的源代码生成器的 API 生成一些模板代码，为了简化书写的代码内容，也为了保持代码文件的整洁。C# 一共提供了两种生成源代码生成器的接口类型：

* `ISourceGenerator`：基本的接口类型，提供基本的源代码生成过程；
* `IIncrementalGenerator`：提供高性能环境下的源代码生成。

本项目会使用到它们来完成源代码的生成过程。

## 前置条件

源代码生成器项目依赖于 C# 语义分析的相关 Nuget 包。详情请参考 **[`Microsoft.Diagnostics.CodeAnalysis` 命名空间](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis)**的具体 API 介绍。它对应的 Nuget 包下载地址在[这里](https://www.nuget.org/packages/Microsoft.CodeAnalysis)。

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
* [禁用值类型的无参构造器](disable-parameterless-ctor)：对于值类型生成无参构造器的禁用代码。
* [自动生成枚举类型相关的执行和路由操作](enum-switch-expr)：生成为枚举类型的所有字段进行路由和处理，构造操作的代码。
* [自动重写 `ref struct` 派生下来的方法代码](ref-struct-default-overrides)：对 `ref struct` 类型生成从 `ValueType` 类型派生下来的方法的重写代码。
* **`GlobalConfigValueGenerator`**：只给源代码生成器提供服务。用于给源代码生成器的项目提供版本号。
* **`StepSearcherOptionsGenerator`**：对实现了 `IStepSearcher` 接口的类型生成默认的 `Options` 属性信息的相关代码。
* **`BitOperationsGenerator`**：对 `BitOperations` 静态类型生成额外的方法，用于扩展比特位的相关处理操作。
