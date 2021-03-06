如何使用程序里给出的 `SudokuHandlingException` 进行合理和有意义的异常处理？首先我们需要介绍一下这个异常到底是用来干什么的。



## 什么是 `SudokuHandlingException`？

`SudokuHandlingException` 是整个解决方案里，但凡出现运行时错误的时候，就必然会抛出的异常类型。极少数情况下，会抛出别的异常（比如 `ArgumentException` 等），不过大多数情况下，只要是运行时错误，就会对应一个具体的错误信息。整个错误信息就可以对应到一个异常的错误代码。我们只需要给出异常的错误码，和必需的参数，就可以指定异常的具体情况，然后抛出。这样的话，我也没有必要为一个具体异常类型单独创建一个异常类（若然，会导致代码不好维护，我确实也不想这么干）。

`SudokuHandlingException` 是一个泛型异常，这个异常可以带有你想给出的任何类型的异常的信息参数，只需要传入即可；当然，如果不需要指定错误信息的参数的话，可以使用无泛型参数的非泛型 `SudokuHandlingException` 异常。

整个异常一共支持三个不同的泛型参数重载的异常类型，分别是没有泛型参数、有一个泛型参数和有两个泛型参数的 `SudokuHandlingException`。一般，它的构造器有如下的一些：

```csharp
public SudokuHandlingException();
public SudokuHandlingException(int errorCode);
public SudokuHandlingException(Exception innerException);
```

要么指定错误码，要么指定内部需要抛出的异常；或者干脆不指定参数，也不指定异常的错误码，默认初始化一个异常对象，然后直接抛出（不过这样的异常是没有错误信息的）。



## 使用和抛出异常

下面我们介绍一下这个异常是如何抛出的。

### 例子 1：数独多解的时候

假设我们在计算的时候，发现数独多解。此时我们可以抛出该异常来表示和告知调用方，题目是多解的。

```csharp
if (++solutionCount > 1)
{
    throw new SudokuHandlingException<SudokuGrid>(errorCode: 101, grid);
}
```

我们可以尝试指定错误码和对应的错误信息参数。注意该异常是泛型的，因此需要指定泛型参数的类型；否则无法确定你实例化参数是否来自于这个带一个泛型参数的泛型异常类型 ``SudokuHandlingException`1``。

> 至于为什么第一个参数是 101，这里我不作介绍。这个数值称为错误码，一个具体错误对应一个具体的错误码（错误码是唯一的）。这些错误码将在后续给出。



### 例子 2：程序集初始化错误的时候

考虑一种情况。在初始化载入程序集的时候，我们可能会使用 C# 9 提供的模块初始化器来对一个模块进行初始化，这可能会对接本地硬盘的序列化的数据，然后读取并导入到程序集里。

但是，如果本地磁盘的数据本身就不存在，或者根本就找不到了，这咋办呢？程序应当是不能正确执行的，因此我们需要抛出一个异常来告知用户，程序无法初始化，并立即退出。

```csharp
const string folderName = "lang";
if (!Directory.Exists(folderName))
{
    throw new SudokuHandlingException<string>(errorCode: 401, folderName);
}

const string fileName = folderName + @"\en-us.dic";
if (!File.Exists())
{
    throw new SudokuHandlingException<string>(errorCode: 401, fileName);
}
```



### 错误码表

下面陈列所有的错误码。

| 错误代码 | 错误信息解释                                                 |
| -------- | ------------------------------------------------------------ |
| 101      | 题目多解。                                                   |
| 102      | 题目无解。                                                   |
| 201      | 题目无法完成，由于某个解题器出现故障，导致某个步骤错误。     |
| 202      | 题目本身有误。和 102 错误不同，此时并不强调题目无解，而是用来表示题目无效才导致题目无法使用和进行其它的功能。 |
| 203      | 题目尚未解出，无法使用此功能（暗示这个功能必须题目可解且已经解出的时候，才可使用）。 |
| 301      | 识别类的基本功能尚未初始化。                                 |
| 302      | 无法通过识别器对指定格子的指定数字，或者说，识别后使得题目填数有冲突。 |
| 303      | Tessaract 无法识别图片。                                     |
| 401      | 程序集初始化错误。请确认初始化必需文件是否存在。             |
| 402      | 调色板模块载入失败。                                         |
| 403      | 资源字典文件载入某字段时找不到而产生错误。                   |
| 501      | 找链的时候，父节点未找到。                                   |
| 601      | 有内部异常抛出。                                             |



## 捕获异常

既然有抛出，当然就需要捕获一些没有必要的异常抛出。由于这个异常是带有错误码的，所以我们需要对应错误码来判断。

判断错误码可以使用 C# 6 的异常筛选器特性来判断：

```csharp
if (!internalInitialize())
{
    // Exit the program.
    return;
}

// Do some other works...

bool internalIniatilize()
{
    try
    {
        // Try to deserialize the resource dictionary.
        DeserializeResourceDictionary("English");
    }
    catch (SudokuHandlingException ex) when (ex.ErrorCode == 401)
    {
        // Failed to initialize.
        return false;
    }
    
    return true;
}
```

注意第 16 行的 `when` 语句。`when` 就是这里想说的异常筛选器。异常筛选器可以在不用进入 `catch` 块就可以判断异常到底是否满足需要捕获的条件。如果条件不成立，异常将不会正确捕获，而是走到下一个 `catch` 块；如果后面没有异常处理了，这个不满足条件的异常就会被抛出。

这里之所以需要判断 `ex` 变量的错误码，是因为我们没有必要直接捕获一个不知道到底是什么错误类型的 `SudokuHandlingException`。因为你并不知道错误是不是真正的反序列化的问题。有可能是别的什么问题导致异常抛出，而它并不影响程序执行的话，这样的捕获依然是不合适的。

另外，更不建议你直接捕获 `Exception`，甚至不写出来（缺省异常捕获类型）。因为一个粗略的类型捕获是危险的，你不论所以然就捕获到一个不知道是啥的异常，这肯定不合理。