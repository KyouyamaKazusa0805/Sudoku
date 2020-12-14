# Priority of Operators

The following table lists the C# operators starting with the highest precedence to the lowest. The operators within each row have the same precedence.

| Level | Operators                                                    | Name                           |
| ----- | ------------------------------------------------------------ | ------------------------------ |
| 1     | `x.y` `x?.y` `x?[y]` `f(x)` `a[i]` `x++` `x--` `new` `typeof` `checked` `unchecked` `default` `nameof` `delegate` `sizeof` `stackalloc` `x->y` `!` | Primary                        |
| 2     | `+x` `-x` `!x` `~x` `++x` `--x` `^x` `(T)x` `await` `&x` `*x` `true` `false` | Unary                          |
| 3     | `x..y`                                                       | Range                          |
| 4     | `switch`                                                     | Switch expression              |
| 5     | `with`                                                       | With expression                |
| 6     | `x * y` `x / y` `x % y`                                      | Multiplicative                 |
| 7     | `x + y` `x – y`                                              | Additive                       |
| 8     | `x << y` `x >> y`                                            | Shift                          |
| 9     | `x < y` `x > y` `x <= y` `x >= y` `is` `as`                  | Relational and type-testing    |
| 10    | `x == y` `x != y`                                            | Equality                       |
| 11    | `x & y`                                                      | Boolean or bitwise logical AND |
| 12    | `x ^ y`                                                      | Boolean or bitwise logical XOR |
| 13    | `x | y`                                                      | Boolean or bitwise logical OR  |
| 14    | `x && y`                                                     | Conditional AND                |
| 15    | `x || y`                                                     | Conditional OR                 |
| 16    | `x ?? y`                                                     | Null-coalescing operator       |
| 17    | `c ? t : f`                                                  | Conditional operator           |
| 18    | `x = y` `x += y` `x -= y` `x *= y` `x /= y` `x %= y` `x &= y` `x |= y` `x ^= y` `x <<= y` `x >>= y` `x ??= y` `=>` | Assignment and lambda          |