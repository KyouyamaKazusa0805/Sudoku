# 函数指针
是的，你没看错，现在 C# 也有函数指针了，下面我们来说一下，C# 里的函数指针到底怎么使用，究竟怎么实现的。

## C 语言的函数指针

C 语言的指针早已经不是新鲜事了，因为我们学编程就知道，C 语言的指针本来就不是人玩的东西。C 语言的指针为什么这么难呢？因为它的使用相当灵活，甚至灵活到创造世间万物，理论上都是可以做到的。

指针是用来表示一个“变量地址”的变量。说白了，它不存储整数、小数、字符、布尔量，而是一个变量的地址。只要我们知道原始变量的类型，我们就可以通过定义，表达出这个指针的数据类型。比如，我们存储的变量本身是 `int` 类型的，那么我们就可以认为，这个指针是 `int*` 类型的，其中的 `*` 则是和普通变量作区分——它不是一个普通变量的记号。

当然，指针并非只能用在变量上。C 语言的复杂程度让我们觉得 C 语言并不好学，它还可以用在数组上，于是就有了数组指针（类似写成 `int(*)[]` 这样的奇葩玩意儿）。

指针甚至可以用在函数上。换句话说，用一个指针变量和存储一个函数的地址。是的，函数也有地址，这个地址数值只是我们平时基本上接触不到罢了。我们试着用一下函数指针。假设我们有一个函数用来对一个数据进行排序。并有一个参数，这个参数是一个函数指针，它表示一个函数，这个函数控制排序的比较关系是怎么做的。

```c
void bubbleSort(int *arr, int length, int (*comparison)(int, int))
{
    for (int i = 0; i < length - 1; i++)
    {
        for (int j = 0; j < length - 1 - i; j++)
        {
            if (comparison(arr[j], arr[j + 1]) >= 0)
            {
                int temp = arr[j];
                arr[j] = arr[j + 1];
                arr[j + 1] = temp;
            }
        }
    }
}
```

是的，写成 `int (*comparison)(int, int)`，左侧的 `int` 表示这个函数原始的返回值类型，而右边的小括号表示这个原始函数的参数是传入两个 `int` 数据。

咋用这个排序函数呢？

```csharp
int main(void)
{
    int arr[] = { 3, 8, 1, 6, 5, 4, 7, 2, 9 };
    
    int (*funcPtr)(int, int) = &compareTwoValue; // Here.
    
    bubbleSort(arr, sizeof(arr) / sizeof(int), funcPtr); // Pass the function pointer.
}

int compareTwoValue(int a, int b)
{
    return a - b;
}
```

这里有三个知识点说一下。第一个是 `int (*funcPtr)(int, int)` 怎么到这里成变量赋值了？第二个是，`sizeof(arr) / sizeof(int)` 是什么？第三个则是，我们这里的 `bubbleSort` 函数既然写出来了，怎么声明函数（没忘吧，C 语言的函数是需要声明的）？

我们一个一个回答。第一个问题。你没看错，这里的 `funcPtr` 是一个指针变量，它指向了一个函数 `compareTwoValue`。既然是指向这个函数，那么赋值的右侧必须就是一个地址数据，因此我们就得把函数名当成一个变量来用，因此，“`&变量` 表示一个地址”这个概念，你应该没有忘记吧。得到结果后，我们就把数据往左边赋值就行了。虽然写成这种格式，但是它确实是一个变量的写法。你想想这个道理：一个函数的地址如果要当成变量来用的话，那么就得有返回值和参数的格式一起写到变量上去吧。那么，格式自然就是这样了。而这里 `funcPtr` 左边的星号是干啥呢？一个函数是不能直接拿来用的，那么就必须用到指针，自然这个符号就得出现了。那么为啥有括号呢？因为 `(*ptr)` 是一个整体，否则这个小括号去掉后会被编译器看成“返回值是 `int*` 的普通函数变量”。显然就不符合我们预期的理解了。

第二个问题，这个是 C 语言计算数组元素个数的办法：`sizeof(数组)` 总是返回整块数组占据内存的字节数，而 `sizeof(int)` 就是每一个元素的所占字节数，因此除法得到的结果就是总元素数。

第三个问题。函数声明是吧，抄一遍函数头就可以了；当然，也可以去掉参数名：

```c
// OK.
void bubbleSort(int *arr, int length, int (*comparison)(int, int));

// Also OK.
void bubbleSort(int *, int, int (*)(int, int));
```

我相信你更喜欢少写点字。但是后面这种就不好看了，因为 `(*)` 初学就是理解不了这个写法；特别是一个星号括起来后还有俩 `int` 小括号括起来。

那么整体代码就比较好理解了：我们用参数表示一个比较函数，它专门表示我们到底怎么在冒泡排序法里比较两个数据的。这个**被指向函数**（Function Pointee）里直接是两数相减，那么嵌入到冒泡排序法里就好比是把 `arr[j]` 和 `arr[j + 1]` 作差，得到的结果如果大于等于 0，就交换这两个数据。这不就是表示 `arr[j] >= arr[j + 1]` 的时候交换吗？被指向函数还可以改成 `return b - a;`，这样就表示结果反过来减，于是这里的比较就成了“当左侧的数比右侧的数小的时候，交换变量”，那么冒泡排序法最终得到的序列就是降序的。因此，被指向函数的功能就显得格外重要：我们不需要提供 `bubbleSort` 的实现，而是通过函数声明暴露给 C 语言使用的用户，这样用户就可以在不知晓函数怎么实现的时候就可以更灵活地控制排序的逻辑，而得到灵活的排序结果，这也是一种良好的封装过程。



## C# 的函数指针

C# 的函数指针由于会兼容 C 语言和 C++ 的函数，因此会有**托管函数**（**托管方法**，Managed Function）和**非托管函数**（**非托管方法**，Unmanaged Function）的概念。

* 托管函数：函数由 C# 语法实现，底层也是用的 C# 提供的 CLR 来完成的。
* 非托管函数：函数并不由 C# 实现，它不受 C# 语法控制，而是通过 DLL 文件交互使用。

### 托管函数的函数指针

那么，我们先来说一下 C# 函数内部的函数指针（托管函数的函数指针）：

```csharp
unsafe
{
    int arr[] = { 3, 8, 1, 6, 5, 4, 7, 2, 9 };
    delegate* managed<int, int, int> funcPtr = &compareTwoValue; // Here.
    bubbleSort(arr, funcPtr); // Pass the function pointer.
}

static int compareTwoValue(int a, int b) => a - b;

static unsafe void bubbleSort(int* arr, delegate* managed<int, int, int> comparison)
{
    for (int i = 0; i < arr.Length; i++)
    {
        for (int j = 0; j < arr.Length - 1 - i; j++)
        {
            if (comparison(arr[j], arr[j + 1]) >= 0)
            {
                int temp = arr[j];
                arr[j] = arr[j + 1];
                arr[j + 1] = temp;
            }
        }
    }
}
```

就是把 C 里的 `int (*ptr)(int, int)` 改成 `delegate* managed<int, int, int>`。先写函数记号 `delegate` 关键字，然后带一个星号。这两个东西是函数指针声明的头部，是固定不变的语法规则。接着，指针符号后面写上 `managed` 关键字，这也是 C# 9 里提供的一个新关键字，它在这里的语义表示一个“托管函数”。然后使用委托的类似语法：尖括号里写类型参数表列，最后一个类型参数是整个函数的返回值类型。如果一个函数没有参数，返回值为 `void` 就写成 `delegate* managed<void>`，如果有多个参数，就把参数挨个写上去，然后返回值上追加一个类型参数在末尾就可以了。

另外，`managed` 默认可以不写，因为 C# 的函数指针默认是指向托管函数的，于是，记号就简化成了 `delegate*<int, int, int>`。当然，你得注意一下，函数指针是不安全的，所以需要先写 `unsafe` 才能用。

接着，我们来说一下非托管函数是怎么用的。

> 如果你对 C# 的**交互性**（也叫**互操作性**，Interoperability）不了解或不太了解的话，这一段文字我是讲不明白的，因为也不重要，因此你在看到这里的话，就可以撤退了，或者直接跳到结尾。

### 非托管函数的函数指针

首先我们来说一下函数**调用约定**（Calling Convention，注意这里是 Convention 不是 Conversion）的概念。函数在底层是有很多实现模式的，而一旦出现大量函数混用的话，C 语言、C++ 甚至 C# 这样的编程语言就得考虑，如何管理这么多函数；函数在调用的时候，应如何遵守约定。

因为函数可以通过指针进行调用，那么问题来了：我们怎么从 C# 里找到 C 和 C++ 里写的函数，并调用它们呢？DLL 文件陈列了很多函数提供给大型项目使用，但函数的调用方式不同，就会意味着函数有不同的执行办法。

#### `__cdecl`

最基础的就是 **C 语言声明的函数**（C Declaration，简称 Cdecl，在代码里记作 `__cdecl` 或 `_cdecl`），它在函数被得到调用的时候，从右往左反向压栈参数，并由**调用方**（Caller）清除**被调用方**（Callee）栈帧。这种机制好处就在于，变长参数可以通过这种模式来实现。

```c
#include <stdio.h>
#include <stdarg.h>
#include <limits.h>

#define END INT_MIN

int __cdecl addRange(...)
{
    va_list ap; // Define variable argument list.
    va_start(ap, first_num); // Initialize the list.

    int result = first_num, temp = 0;
    
    // Get values.
    while ((temp = va_arg(ap, int)) != END)
    {
        result += temp;
    }

    va_end(ap); // Close and clean the values.

    return result;
}

int main(void)
{
    int result = addRange(1, 2, 4, 8, 16, 32, END);
    printf("%d\n", result);
}
```

为什么呢？因为变长参数需要借助一个叫 `va_list`、`va_end` 这样的东西来辅助实现。如果函数执行完毕就清栈的话，变长参数由于存储在堆内存里，因而得不到内存释放，而且栈帧也被清除，所以就无法再找到它们的内存空间了，这就是我们俗称的**内存泄漏**（Memory Leak）问题。

> 当然，`__cdecl` 是 C 语言和 C++ 默认的调用约定，因而可以省略；换句话说，缺省时默认函数就是这种调用约定的。

#### `__fastcall`

这是一种函数调用模型，再来说一个：`__fastcall`。这个和前文的调用模型有所不同的地方是，函数在执行完毕后立马清栈，然后才会返回到被调用方。显然，变长参数就无法使用这种调用模型了。而之所以名字带 fast，是因为它有一个寄存器存储的处理：它会把参数表列从左往右看的前两个不大于 4 字节（两个 DWORD）的参数直接丢进寄存器 ECX 和 EDX 里。学过计算机组成原理的朋友们都知道，寄存器是 CPU 里的一个部件，它离 ALU（运算器）最近，因此即存即取的操作使得运算代码会相当快。但是，因为寄存器很小，因此不能放很多数据进去。

#### `__stdcall`

这个是专门用在 Win32 API 里的一种调用模型，就不介绍那么多了（它也是从右到左反向压栈参数）。

#### 用法

常见的调用约定一共有 `__cdecl`、`__fastcall` 和 `__stdcall` 这样三种，C 语言里把这三个修饰符放在返回值和函数名中间，比如前面的 `int __cdecl addRange` 就是这样的写法。

#### 函数指针和调用约定

下面说一下函数指针和这些调用约定怎么进行混用。我们拿 C 语言一个常见的排序函数 `qsort` 举例。这个函数最适合这里解释和介绍函数指针的内容，因为它的第四个参数就是一个必须指向 `__cdecl` 这样调用约定的函数指针。

我们先看看它在 C 语言里的声明：

```c
void qsort(
    void *base,
    size_t number,
    size_t width,
    int (__cdecl *compare)(const void *, const void *)
);
```

首先，我们要注意的是 `size_t`。这个 `size_t` 是一个类型别名，在64位系统中为 `long long unsigned int`，非64位系统中为 `long unsigned int`。需要注意的是，C# 里的 `int` 和 C 的 `long` 一样大；而 C 里的 `int` 是不定长的，因而不能直接和 C# 的 `int` 进行大小比较。那么，既然这么说了，那么 `long long` 就等价于 C# 的 `long`，而 `unsigned` 就是 C# 里类型的 `u` 前缀，故就是 `ulong` 类型；而同理可得，非 64 位系统则是 `uint` 了。是的，这是不定长的数据表达；当然，C# 9 里提供了一种新类型 `nuint` 来专门表达这种平台不定长的数据类型，因此可以直接在 C# 代码里体现和代换成 `nuint`。

最后一个参数 `int (__cdecl *compare)(const void *, const void *)` 就是我们熟知的函数指针了。它指向一个声明格式是 `int func(const void *a, const void *b)` 的函数，即带两个 `const void *`，并返回 `int` 类型的函数。

> 稍微注意下，函数指针上有 `__cdecl` 修饰符，这表示，被指向函数必须用 `__cdecl` 这种调用约定才可以；换句话说，随意一个函数传过去都是不允许的。

问题来了。我们要想调用带这个函数的 DLL 文件，我们写到 C# 里必然是使用 `extern` 关键字修饰，并使用 `DllImportAttribute` 的。那么，我们怎么写代码呢？

我们假设 `MSVCP60.dll` 文件里包含这个函数，我们在写 C# 代码的时候就需要这么写：

```csharp
[DllImport("MSVCP60", CallingConvention = CallingConvention.Cdecl)]
static unsafe extern void qsort(
    void* arr,
    nuint nitems,
    nuint size,
    delegate* unmanaged[Cdecl]<void*, void*, int> comparison
);
```

请注意第四个参数的格式。我们需要把前文讲到的 `managed` 关键字替换为 `unmanaged`，因为这里的函数指针要契合 `qsort` 函数来排序，而 `qsort` 函数声明里带 `__cdecl` 修饰符，因而这个函数指针仅能使用到 `__cdecl` 这样调用转换的函数上。在写成 C# 的时候，这个函数指针需要写成 `unmanaged[Cdecl]`。`Cdecl` 是 C# 里一个叫做 `CallConvCdeclAttribute` 的特性的简写。因为名字过长，所以 C# 就让你去掉 `CallConv` 前缀和 `Attribute` 后缀，最后就只保留了 `Cdecl` 这一截。所以，你不必问我为什么 `__cdecl` 在 C# 里写成 `Cdecl` 了，因为这就是原因。

另外，函数除了函数指针这个参数用了 `Cdecl` 修饰外，函数本身也是 C 语言里的库函数，因此它本身也是满足 `__cdecl` 调用转换模型的，不过这里不是函数指针，因此没有前文那样的语法。这里因为需要引入 DLL 文件，因此有了 `DllImport` 特性的修饰。首先，第一个参数传入的是文件名。前文假设是 `MSVCP60.dll` 文件里包含这个函数，因此这里写 `MSVCP60.dll` 或者 `MSVCP60`。接着，调用约定需要手动指明，因为不说的话，C# 不知道是什么调用约定，这里的调用约定是用的枚举，因此写成 `CallingConvention.Cdecl` 的格式。

接着，我们使用这个函数。说一下四个参数的作用。第一个参数就是指向的数组，因为 `qsort` 是支持任意数据类型的数据参与比较的（比较的操作就从第四个参数来指明），所以是 `void*` 类型；第二个参数表示数据有多少个；第三个参数表示数据的每一个元素都占据多少个内存空间大小。这里刚好可以用 C 语言里的 `sizeof` 来表示，所以不用担心怎么手动计算；最后一个参数前文说了，指明比较到底是怎么操作的。

说明完毕后，我们可以开始调用这个函数。

```csharp
unsafe
{
    int[] arr = { 3, 8, 1, 6, 5, 4, 7, 2, 9 };

    delegate*<void*, nuint, nuint, delegate* unmanaged[Cdecl]<void*, void*, int>, void> p = &qsort;

    fixed (int* pArr = arr)
    {
        p(pArr, 9, sizeof(int), &cmp);
    }

    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
    static int cmp(void* l, void* r) => *(int*)l - *(int*)r;
}
```

首先，我们定义一个需要排序的数组，然后，使用 `fixed` 语句固定数组，将数组转成地址形式表示，然后传入函数指针 `p`。

这个 `p` 的类型有点长，但是看得出来它就是一个函数指针。需要注意的是，整体函数指针类型不需要用 `unmanaged` 修饰符修饰，因为它现在已经写在 C# 里面了，即使写了调用约定，也是如此，因此声明并非是

```csharp
delegate* unmanaged[Cdecl]<void*, nuint, nuint, delegate* unmanaged[Cdecl]<void*, void*, int>, void>
```

或

```csharp
delegate* unmanaged<void*, nuint, nuint, delegate* unmanaged[Cdecl]<void*, void*, int>, void>
```

而是

```csharp
delegate*<void*, nuint, nuint, delegate* unmanaged[Cdecl]<void*, void*, int>, void>
```

或

```csharp
delegate* managed<void*, nuint, nuint, delegate* unmanaged[Cdecl]<void*, void*, int>, void>
```

然后，我们在最下方定义比较函数 `cmp`，函数专门用来比较两个数字的大小。先将数字转成普通类型（因为现在还是 `void*`，无法参与比较），然后再相减。差值大于 0，则表示左边比右边大；差值小于 0，则表达右边比左边大；如相同，就表示两个数一样大。

最后，我们传参的时候，把 `cmp` 传过去就可以了。

不过，这样会报错，提示函数 `cmp` 不兼容声明的 `qsort` 的函数指针参数。这是为啥呢？我们没设定 `cmp` 函数的调用约定。是的，这个函数仅给非托管函数 `qsort` 用，所以我们需要先标记一个 C# 9 里带来的新 API：`UnmanagedCallersOnlyAttribute`。这个特性标记出来有两个目的：

* 告诉编译器，这个函数仅提供给非托管函数调用；
* 告诉编译器，这个函数的调用约定是 `__cdecl` 模式的。

是的，我们在该特性上指定调用约定 `Cdecl`，怎么指定呢？写上 `CallConvs = new[] { typeof(CallConvCdecl) }` 就可以了。赋值方写的是调用约定的模型在 C# 里规定的写法。这里传入 `typeof(CallConvCdecl)` 表示我这里是用的 `Cdecl` 模式的调用约定，而我们无法直接传入类名当参数，所以这里用到了 `typeof` 关键字。

写上这个之后，编译器也知道这个函数无法随便用了，因此这样一来，我们就可以成功传参到上面的函数里当函数指针了，这一次就没有编译器错误了。

另外，我们其实都知道，这个根据调用平台来确定函数的调用约定，因此我们其实可以省略这里的 `__cdecl`，所以，代码整体这么写其实也没问题：

```csharp
using System;
using System.Runtime.InteropServices;

unsafe
{
    int[] arr = { 3, 8, 1, 6, 5, 4, 7, 2, 9 };

    delegate*<void*, nuint, nuint, delegate* unmanaged<void*, void*, int>, void> p = &qsort;

    fixed (int* pArr = arr)
    {
        p(pArr, 9, sizeof(int), &cmp);
    }

    [UnmanagedCallersOnly]
    static int cmp(void* l, void* r) => *(int*)l - *(int*)r;

    for (int i = 0; i < 9; i++)
    {
        Console.Write(arr[i]);
    }
}

[DllImport("MSVCP60", CallingConvention = CallingConvention.Cdecl)]
static unsafe extern void qsort(
    void* arr,
    nuint nitems,
    nuint size,
    delegate* unmanaged<void*, void*, int> comparison
);
```

注意去掉的三处地方。第一个是函数指针 `p` 的类型，第四个参数的 `[Cdecl]` 没了；第二个是 `[UnmanagedCallersOnly]` 的 `CallConv` 属性赋值表达式没了；第三个是下方函数 `qsort` 声明的第四个参数里，`[Cdecl]` 没了。去掉这些，编译器就会自动检测和确定，这里 `qsort` 属于什么平台，用什么平台下的默认调用转换了。之前就说过，C 语言默认的函数就是 `__cdecl` 的，所以我们就不必写这些东西了，缺省就是 `__cdecl`。

#### 啰嗦一下 `UnmanagedCallersOnlyAttribute`

这个特性前文已经说到了它的标记目的，那么下面来说一下这个特性到底有哪些需要注意的地方。

第一，**被标记方法只能是 `static` 的**。思考一点，要想用函数指针，就必须方法能够兼容底层 C/C++ 的代码。显然，C 是没有实例方法这种概念的，这是面向对象的东西。虽然 C++ 里有此概念，但它的实现和 C# 的依旧不同。如果我们允许实例方法作为函数指针使用的话，这必然会带来调用和兼容的问题。因此，C# 目前限制函数指针仅用于静态方法。

第二，**被标记函数的返回值和参数类型都必须是 C/C++ 里原生支持的基本数据类型**。换句话说，你不能使用 C# 里定义的数据类型作为这个函数的参数，这样 C/C++ 找不到这个玩意儿，于是就没办法兼容。另外，我们可以把这一点简化一下说法。C/C++ 底层支持的这些基本数据类型因为是**由机器类型来确定的**（Platform-specific），因此也被称为**本机类型**（Blittable Type）；当然，C# 里自定义的结构体啊、类啊、接口之类的东西就称为**非本机类型**（Non-blittable Type）了。

第三，**不能手动调用这些函数**。这显然是废话，因为这个方法专用于底层交互，自然就不能允许我们在 C# 平台里使用了。



## 总结

欢迎你来到本文的最后一节。前文说的东西有些乱，我之后录视频可能还会讲一遍。

实际上，总结我也不知道写些什么，至少你得懂，函数指针是什么玩意儿。



## 参考资料

[1] https://devblogs.microsoft.com/dotnet/improvements-in-native-code-interop-in-net-5-0/

[2] https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.unmanagedcallersonlyattribute?view=net-5.0

[3] https://docs.microsoft.com/en-us/dotnet/framework/interop/blittable-and-non-blittable-types