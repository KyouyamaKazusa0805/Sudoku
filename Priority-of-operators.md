# Priority of Operators

The following table lists the C# operators starting with the highest precedence to the lowest.
The operators within each row have the same precedence.

| No.  | Operators                                                    | Name                                       |
| ---- | ------------------------------------------------------------ | ------------------------------------------ |
| 1    | x.y<br/>x?.y<br/>x?[y]<br/>f(x)<br/>a[i]<br/>x++<br/>x--<br/>new<br/>typeof<br/>checked<br/>unchecked<br/>default<br/>nameof<br/>delegate<br/>sizeof<br/>stackalloc<br/>x->y | Primary                                    |
| 2    | +x<br/>-x<br/>!x<br/>~x<br/>++x<br/>--x<br/>^x<br/>(T)x<br/>await<br/>&x<br/>*x | Unary                                      |
| 3    | true<br/>false                                               | Boolean Unary                              |
| 4    | x..y                                                         | Range                                      |
| 5    | x * y<br/>x / y<br/>x % y                                    | Multiplicative                             |
| 6    | x + y<br/>x – y                                              | Additive                                   |
| 7    | x << y<br/>x >> y                                            | Shift                                      |
| 8    | x < y<br/>x > y<br/>x <= y<br/>x >= y<br/>is<br/>as          | Relational and type-testing                |
| 9    | x == y<br/>x != y                                            | Equality                                   |
| 10   | x & y                                                        | Boolean logical AND or bitwise logical AND |
| 11   | x ^ y                                                        | Boolean logical XOR or bitwise logical XOR |
| 12   | x \| y                                                       | Boolean logical OR or bitwise logical OR   |
| 13   | x && y                                                       | Conditional AND                            |
| 14   | x \|\| y                                                     | Conditional OR                             |
| 15   | x ?? y                                                       | Null-coalescing operator                   |
| 16   | c ? t : f                                                    | Conditional operator                       |
| 17   | x = y<br/>x += y<br/>x -= y<br/>x *= y<br/>x /= y<br/>x %= y<br/>x &= y<br/>x \|= y<br/>x ^= y<br/>x <<= y<br/>x >>= y<br/>x ??= y<br/>=> | Assignment and lambda declaration          |



