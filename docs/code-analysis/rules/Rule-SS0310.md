## 基本信息

**错误编号**：`SS0310`

**错误叙述**：

* **中文**：经 `where` 语句筛选后的序列只有 '{0}' 这一种情况，因此后续对同样字段进行排序是没有意义的。
* **英文**：The selection after `where` clause filtered can only contain only one case '{0}', so the ordering by the same field after the filtered selection is unmeaningful.

**级别**：编译器警告

**警告级别**：1

**类型**：性能（Performance）

## 描述

如果在某处包含 `where` 语句，后紧跟了一个 `orderby` 语句，且这个 `orderby` 语句和 `where` 判别条件是一致的字段，并且 `where` 包含的筛选条件是 `==` 连接的的话，那么显然后面就不需要排序了，因为这个同字段的集合被筛选后，所有的对象的这个字段都是相同的值，对这个字段进行排序就显得完全没有意义。

```csharp
int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

_ =
    from x in arr
    orderby x ascending
    where x == 3
    orderby x ascending // SS0310.
    select x;
```

比如例子里 `where x == 3` 筛选了集合，将所有集合里元素为 3 的元素全部提取出来了；但后面又根据这个前面的集合对 `x` 排序。因为 `x` 自身就只能是 3，那么怎么都没有必要去排序了，因为只有 3 的元素不管你怎么去排序都是 3 的序列。

