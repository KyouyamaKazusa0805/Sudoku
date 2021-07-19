using System;
using System.Text;

using ValueStringBuilder sb = new(stackalloc char[10]);
sb.Append("Hello");
sb.Append('!');
Console.WriteLine(sb.ToString());

using (ValueStringBuilder sb2 = new(stackalloc char[10]))
{
	sb2.Append("Hello");
	sb2.Append('!');
	Console.WriteLine(sb2.ToString());
}
