# Priority of Operators

标题：**运算符的优先级**

The following table lists the C# operators starting with the highest precedence to the lowest.
The operators within each row have the same precedence.

下方的表格列举了 C# 的运算符，以优先级高的在前、低的在后的顺序罗列出来。同一个编号的运算符优先级相同。

| No.<br/>编号  | Operators<br/>运算符                                                    | Name<br/>名称                                       |
| ---- | ------------------------------------------------------------ | ------------------------------------------ |
| 1    | `x.y`<br/>`x?.y`<br/>`x?[y]`<br/>`f(x)`<br/>`a[i]`<br/>`x++`<br/>`x--`<br/>`new`<br/>`typeof`<br/>`checked`<br/>`unchecked`<br/>`default`<br/>`nameof`<br/>`delegate`<br/>`sizeof`<br/>`stackalloc`<br/>`x->y` | Primary<br/>基本运算符                                    |
| 2    | `+x`<br/>`-x`<br/>`!x`<br/>`~x`<br/>`++x`<br/>`--x`<br/>`^x`<br/>`(T)x`<br/>`await`<br/>`&x`<br/>`*x` | Unary<br/>单目运算符                                      |
| 3    | `true`<br/>`false`                                               | Boolean Unary<br/>单目布尔运算符                              |
| 4    | `x..y`                                                         | Range<br/>范围运算符                                      |
| 5    | `x * y`<br/>`x / y`<br/>`x % y`                                    | Multiplicative<br/>乘除运算符                             |
| 6    | `x + y`<br/>`x – y`                                              | Additive<br/>加减运算符                                   |
| 7    | `x << y`<br/>`x >> y`                                            | Shift<br/>移位运算符                                      |
| 8    | `x < y`<br/>`x > y`<br/>`x <= y`<br/>`x >= y`<br/>`is`<br/>`as`          | Relational and type-testing<br/>关系运算符和类型测试                |
| 9    | `x == y`<br/>`x != y`                                            | Equality<br/>相等判别运算符                                   |
| 10   | `x & y`                                                        | Boolean logical AND or bitwise logical AND<br/>逻辑与和位与运算符 |
| 11   | `x ^ y`                                                        | Boolean logical XOR or bitwise logical XOR<br/>逻辑异或和位异或运算符 |
| 12   | `x | y`                                                       | Boolean logical OR or bitwise logical OR<br/>逻辑或和位或运算符   |
| 13   | `x && y`                                                       | Conditional AND<br/>条件与运算符                            |
| 14   | `x || y`                                                     | Conditional OR<br/>条件或运算符                             |
| 15   | `x ?? y`                                                       | Null-coalescing operator<br/>null 合并运算符                   |
| 16   | `c ? t : f`                                                    | Conditional operator<br/>条件运算符                       |
| 17   | `x = y`<br/>`x += y`<br/>`x -= y`<br/>`x *= y`<br/>`x /= y`<br/>`x %= y`<br/>`x &= y`<br/>`x |= y`<br/>`x ^= y`<br/>`x <<= y`<br/>`x >>= y`<br/>`x ??= y`<br/>`=>` | Assignment and lambda declaration<br/>赋值和 Lambda 表达式声明运算符          |



