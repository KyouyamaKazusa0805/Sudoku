using System;
using System.Runtime.CompilerServices;
using System.Text;

//
// string.Format
//
string a = "Hello";
char b = ',';
string c = "world";
char d = '!';
Console.WriteLine($"{a}{b}{c}{d}");

//
// Imrpoved interpolated string
//
var s = new DefaultInterpolatedStringHandler();
s.AppendFormatted("Hello");
s.AppendFormatted(',');
s.AppendFormatted("world");
s.AppendFormatted('!');
Console.WriteLine(s.ToStringAndClear());

//
// ValueStringBuilder
//
var v = new ValueStringBuilder();
v.Append("Hello");
v.Append(',');
v.Append("world");
v.Append('!');
Console.WriteLine(v.ToString());