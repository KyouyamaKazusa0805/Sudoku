using System.Linq;

int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

_ = from x in arr orderby x, x select x;
_ = from x in arr orderby x, x descending select x;