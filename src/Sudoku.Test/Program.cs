using System;
using System.Linq;
using System.Numerics;

Console.WriteLine("\t\tpublic static readonly int[,] Combinatorials =");
Console.WriteLine("\t{");
CreateCollection();
Console.WriteLine("\t};");

static partial class Program
{
	private static readonly BigInteger Max = int.MaxValue;

	public static void CreateCollection()
	{
		foreach (int n in Enumerable.Range(1, 30))
		{
			int[] list = new int[30];
			Array.Fill(list, -1);
			foreach (int m in Enumerable.Range(1, n))
			{
				var result = f(n) / (f(m) * f(n - m));
				list[m - 1] = result <= Max ? (int)result : -1;
			}

			Console.Write("\t\t{ ");
			Console.Write(string.Join(", ", list));
			Console.WriteLine(" },");
		}


		static BigInteger f(int n)
		{
			if (n == 0) return BigInteger.One;

			var result = BigInteger.One;
			for (int i = 1; i <= n; i++)
			{
				result *= i;
			}

			return result;
		}
	}
}
