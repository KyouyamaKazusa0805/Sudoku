# SD0415
## 基本信息

**错误编号**：`SS0415`

**错误叙述**：

* **中文**：标记了 `[RefStructType]` 特性的泛型参数传入的参数可能会导致装箱。
* **英文**：The type parameter marked `[RefStructType]` can't be used here because here may cause a box operation.

**级别**：编译器错误

**类型**：使用（Usage）

## 描述

为了保证这个泛型参数确实是一个不装箱的 `ref struct`，它不能以任何方式装箱。举个例子。

```csharp
void Append<[RefStructType] TRefStruct>(TRefStruct type) where TRefStruct : unmanaged
{
    //                            ↓
    _innerBuilder.Append(type.ToString());
}
```

如果调用方传入的泛型参数自己不带有 `ToString` 方法的话，就会产生装箱行为。
