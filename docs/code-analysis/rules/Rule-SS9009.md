# SS9009
## 基本信息

**错误编号**：`SS9009`

**错误叙述**：

* **中文**：建议使用 `ValueTask<T>` 代替 `Task<T>`。
* **英文**：We suggest you replace `Task<T>` with `ValueTask<T>`.

**级别**：编译器信息

**类型**：性能（Performance）

## 描述

C# 的 `ValueTask<T>` 一般在非常轻量级的异步操作里会使用到，这样可以节省内存分配来节约性能。`Task<T>` 内存分配较大，所以被定义为了引用类型；而 `ValueTask<T>` 专门用于表达一些简单的操作和行为，因此比较节省性能。但是有一个比较麻烦的地方是，`ValueTask<T>` 不支持取消，因为它的构造器没有像 `Task<T>` 一样的、可传入 `CancellationToken` 的参数的构造器。所以注意取舍。

下面列举一个可用 `ValueTask<T>` 代替 `Task<T>` 的例子。

```csharp
public Task<int> GetValueAsync()
{
    return new(getResultSample);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int getResultSample() => 42;
}
```

如代码所示，在返回结果的时候，我们使用了 `new Task` 的语法来把普通方法转换为异步方法。这是我们常见的转换方式（虽然这一点来说从底层有点不好，不建议这么用）。

不过，因为它的参数转换使用的是 Lambda 表达式，且直接是一个超级简单的返回结果的表达式，因此我们可通过使用 `ValueTask<T>` 的构造器（传入结果的构造器）来完成这个任务，这样代码就会简单特别多。

```csharp
public ValueTask<int> GetValueAsync()
{
    return new(42);
}
```

甚至是写成表达式体的方式：

```csharp
public ValueTask<int> GetValueAsync() => new(42);
```

`ValueTask<T>` 里是自带一个构造器，直接可传入一个结果作为对象实例化的参数。它基本上就等价于前面的 `Task<T>` 的处理机制，但更简单、更轻量级。但一定需要注意的是，这样处理就不支持取消了。如果原始操作是这么写的话：

```csharp
public Task<int> GetValueAsync(CancellationToken cancellationToken = default)
{
    return new(getResultSample, cancellationToken);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    static int getResultSample() => 42;
}
```

因为机制的关系，`ValueTask<T>` 从构造器上不支持取消，因此不能这么做。

## 备注

请注意这里两个例子的方法的签名，签名是不带 `async` 和 `await` 的。返回的这个 `Task<T>` 和 `ValueTask<T>` 因为底层直接支持 `async` 异步处理机制，因此可能你会这么写代码：

```diff
- public Task<int> GetValueAsync()
+ public async Task<int> GetValueAsync()
  {
-     return new(getResultSample);
+     return await new(getResultSample);

      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      static int getResultSample() => 42;
  }
```

请注意。有没有 `async`/`await` 在执行上是有区别的。如果没有，那么任务对象会作为对象形式返回；但带有 `async`/`await` 的方法，因为底层会把它翻译成状态机和普通的、返回 `int` 的方法，而底层的状态机会在方法某处开始异步执行，因此处理机制不同。
