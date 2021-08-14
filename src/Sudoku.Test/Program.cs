using System.Runtime.CompilerServices;

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