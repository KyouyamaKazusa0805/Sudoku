# 代码分析器

## 介绍

这个项目甚至提供了对代码进行分析的项目。

| 分析编号               | 类型                     | 级别[^1] | 信息内容                                                     |
| ---------------------- | ------------------------ | -------- | ------------------------------------------------------------ |
| [SCA0001](sca0001)[^2] | 代码分析（CodeAnalysis） | ❌        | 特殊类型缺失                                                 |
| [SCA0002](sca0002)     | 代码生成（CodeGen）      | ❌        | 代码分析器缺失特性标记                                       |
| [SCA0003](sca0003)     | 代码生成（CodeGen）      | ⚠        | `RegisteredPropertyNamesAttribute` 的第二个参数必须至少有一个值 |
| [SCA0004](sca0004)     | 代码生成（CodeGen）      | ⚠        | 代码分析器类型应用 `sealed` 修饰，并且不能用 `abstract` 修饰 |
| [SCA0005](sca0005)     | 代码生成（CodeGen）      | ❌        | `RegisterOperationActionAttribute` 不支持该类型内容的分析    |
| [SCA0006](sca0006)     | 代码生成（CodeGen）      | ⚠        | 代码分析器名称命名不规范                                     |
| [SCA0101](sca0101)     | 使用（Usage）            | ⚠        | 请勿对大结构类型进行无参实例化                               |
| [SCA0102](sca0102)     | 性能（Performance）      | ⚠        | 大结构类型的参数需要按引用传递                               |
| [SCA0103](sca0103)     | 使用（Usage）            | ❌        | 该构造器预留给编译器和运行时调用，用户不应调用此成员         |
| [SCA0104](sca0104)     | 使用（Usage）            | ⚠        | `Equals` 方法位于 `ref struct` 类型的重写，无论如何都无法成立；调用此成员无意义 |
| [SCA0105](sca0105)     | 使用（Usage）            | ⚠        | `GetHashCode` 方法位于 `ref struct` 类型尚未正确重写，无论如何都无法得到正确结果；调用此成员无意义 |
| [SCA0106](sca0106)     | 使用（Usage）            | ❌        | 该函数指针类型字段不可被调用                                 |
| [SCA0107](sca0107)     | 使用（Usage）            | ⚠        | `DisallowFunctionPointerInvocationAttribute` 特性不应标记到非函数指针类型的字段上；没有意义 |
| [SCA0108](sca0108)     | 使用（Usage）            | ⚠        | 该构造器不应用户调用；没有意义                               |
| [SCA0109](sca0109)     | 设计（Design）           | ❌        | 标记了 `FileAccessOnlyAttribute` 的字段只能在当前文件内可用  |
| [SCA0110](sca0110)     | 设计（Design）           | ⚠        | 标记了 `FileAccessOnlyAttribute` 的字段不能是 `private` 的   |
| [SCA0111](sca0111)     | 设计（Design）           | ❌        | 标记了 `FileAccessOnlyAttribute` 的构造器只能在当前文件内可用 |
| [SCA0112](sca0112)     | 设计（Design）           | ℹ        | 无需为 `file` 修饰符修饰的类型的成员使用 `FileAccessOnlyAttribute` 特性 |
| [SCA0113](sca0113)     | 使用（Usage）            | 💡        | 该泛型参数缺少 `SelfAttribute` 特性                          |
| [SCA0114](sca0114)     | 使用（Usage）            | ⚠        | 该泛型参数必须限制自身约束                                   |
| [SCA0115](sca0115)     | 命名（Naming）           | ℹ        | Self 泛型参数应命名为 `TSelf`                                |
| SCA0201                | 使用（Usage）            | ℹ        | 请使用 `Argument` 类型代替 `if` 判断                         |
| [SCA0202](sca0202)     | 性能（Performance）      | ℹ        | 请使用 `Add` 方法代替 `operator +` 运算符                    |
| [SCA0203](sca0203)     | 性能（Performance）      | ℹ        | 请使用 `Remove` 方法代替 `operator -` 运算符                 |
| [SCA0204](sca0204)     | 设计（Design）           | ⚠        | 请不要在调用了 `ToStringAndClear` 之后仍使用同一对象         |
| [SCA0205](sca0205)     | 设计（Design）           | ℹ        | 请使用 `ToStringAndClear` 释放内存；请勿使用 `using` 关键字对该对象使用默认的释放行为 |
| [SCA0206](sca0206)     | 使用（Usage）            | ℹ        | 请使用强制转换代替 `Grid.Parse` 方法                         |
| [SCA0207](sca0207)     | 使用（Usage）            | ℹ        | 请使用 `Grid.Parse` 方法代替强制转换                         |

## 注解

[^1]: 分析级别分四种：隐藏（💡）、信息（ℹ）、警告（⚠）和错误（❌），其中编译器错误会直接禁止编译对代码进行编译，属于严重错误（诸如代码语法错误等问题则属于这一类错误）。
[^2]: 所有以“SCA00”起头的代码分析规则都是源代码生成器内部提供的分析。由于 C# 自身的限制，所有这些规则的级别只可能是警告（⚠图标）或错误（❌图标）这两种情况。
