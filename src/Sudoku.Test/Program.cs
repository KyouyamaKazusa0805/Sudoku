using System.Linq;

int[] s = { 1, 3, 45, 6, 8 };

_ = from e in s where true select e;
_ = from e in s where false select e;
_ = from e in s select e + 10;