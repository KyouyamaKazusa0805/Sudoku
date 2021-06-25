using System;
using System.Collections.Generic;
using System.Linq;

int[] arr = { 1, 3, 5, 7, 9, 11, 13, 15, 20 };

int element = arr.ElementAt(3);
int element2 = arr.ElementAt(^1);

IEnumerable<int> selection = arr;

int elementAnother = selection.ElementAt(3);
int elementAnother2 = selection.ElementAt(^1);

Console.WriteLine(element);
Console.WriteLine(element2);
Console.WriteLine(elementAnother);
Console.WriteLine(elementAnother2);