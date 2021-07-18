using System;
using System.Text;

var vsb = new ValueStringBuilder(stackalloc char[10]);
var vsb2 = new ValueStringBuilder(stackalloc char[10]);
var vsb3 = new ValueStringBuilder(stackalloc char[10]);

Console.WriteLine(vsb.ToString());
vsb.Append('!'); // Wrong.
vsb.Append('!'); // Wrong.
int i = 30;
Console.WriteLine(i);
vsb.Append('!'); // Wrong.
vsb.Append('!'); // Wrong.

vsb2.Append(vsb2.ToString()); // Wrong.

vsb3.Append('!');
Console.WriteLine(vsb3.ToString());