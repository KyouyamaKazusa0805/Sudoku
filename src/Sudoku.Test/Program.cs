using System;
using System.Collections.Generic;
using System.Linq;

var list = new List<int> { 1, 3, 5, 7, 9, 11, 13, 15, 17, 19 };
var selection = from item in list where (item & 1) != 0 select item;
if (selection.Take(5).Count() > 5 && list.Count >= 5)
{
	Console.WriteLine(list[4]);
}