# 源代码生成器简介

## 介绍

该项目使用 C# 9 和 C# 10 提供的源代码生成器的 API 生成一些模板代码，为了简化书写的代码内容，也为了保持代码文件的整洁。C# 一共提供了两种生成源代码生成器的接口类型：

* `ISourceGenerator`：基本的接口类型，提供基本的源代码生成过程（现已被微软官方弃用）；
* `IIncrementalGenerator`：提供高性能环境下的源代码生成。

本项目会使用到它们来完成源代码的生成过程。

## 前置条件

源代码生成器项目依赖于 C# 语义分析的相关 Nuget 包。详情请参考 **[`Microsoft.Diagnostics.CodeAnalysis` 命名空间](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis)**的具体 API 介绍。它对应的 Nuget 包下载地址在[这里](https://www.nuget.org/packages/Microsoft.CodeAnalysis)。

## 源代码生成器

下面列举所有的源代码生成器。

* **`GeneratedDeconstructionGenerator`**：生成解构函数的代码。
* **`GeneratedExtensionDeconstructionGenerator`**：生成扩展解构函数的代码。
* **`DefaultOverriddenMembersGenerator`**：生成对 `object` 自带的虚成员的重写默认实现。
* **`AutoOverloadingOperatorGenerator`**：生成对运算符重载的默认实现。
* [自动属性绑定](property-binding)：为 UI 项目生成字段绑定属性的 `PropertyChanged` 事件触发和赋值判断代码。
* **`DependencyPropertyGenerator`**：为 UI 项目提供依赖属性的代码。
* **`AttachedPropertyGenerator`**：为 UI 项目提供附着（附加）属性的代码。
* **[自动生成枚举类型相关的执行和路由操作](enum-switch-expr)**：生成为枚举类型的所有字段进行路由和处理，构造操作的代码。
* **`VersionValueGenerator`**：只给源代码生成器提供服务。用于给源代码生成器的项目提供版本号。
* **`StepSearcherOptionsGenerator`**：对实现了 `IStepSearcher` 接口的类型生成默认的 `Options` 属性信息的相关代码。
* **`BitOperationsGenerator`**：对 `BitOperations` 静态类型生成额外的方法，用于扩展比特位的相关处理操作。
* **`LogicalSolverOperationsGenerator`**：对 `LogicalSolver` 生成一些在派生类型下使用的配置性属性（即标记了 `SearcherPropertyAttribute` 特性）的关联属性。

