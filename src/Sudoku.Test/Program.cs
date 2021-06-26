using System;
using System.Linq;

string[] strings = { "1", "2", "3" };
var stringsAsObjects = from @string in strings select (object)@string;
Console.WriteLine(stringsAsObjects);