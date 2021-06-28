using System.Linq;

int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

_ = from x in arr where x / 2 == 0 where x >= 8 select x;