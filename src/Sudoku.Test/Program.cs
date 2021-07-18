using System;
using System.Text;

ValueStringBuilder vsb = new(stackalloc char[10]), vsb2 = new(stackalloc char[10]), vsb3 = new(stackalloc char[10]);

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

class MyClass
{
	void F()
	{
		ValueStringBuilder vsb = new(stackalloc char[10]), vsb2 = new(stackalloc char[10]), vsb3 = new(stackalloc char[10]);

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
	}

	void G()
	{
		g();

		static void g()
		{
			ValueStringBuilder vsb = new(stackalloc char[10]), vsb2 = new(stackalloc char[10]), vsb3 = new(stackalloc char[10]);

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
		}
	}
}