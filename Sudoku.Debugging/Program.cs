/////////////////////////////////////////////////////////
// ███████╗██╗   ██╗██████╗  ██████╗ ██╗  ██╗██╗   ██╗ //
// ██╔════╝██║   ██║██╔══██╗██╔═══██╗██║ ██╔╝██║   ██║ //
// ███████╗██║   ██║██║  ██║██║   ██║█████╔╝ ██║   ██║ //
// ╚════██║██║   ██║██║  ██║██║   ██║██╔═██╗ ██║   ██║ //
// ███████║╚██████╔╝██████╔╝╚██████╔╝██║  ██╗╚██████╔╝ //
// ╚══════╝ ╚═════╝ ╚═════╝  ╚═════╝ ╚═╝  ╚═╝ ╚═════╝  //
/////////////////////////////////////////////////////////
// Here is Sunnie's debugging room!

// Global suppressions.
#pragma warning disable IDE0001
#pragma warning disable IDE0002
#pragma warning disable IDE0005
#pragma warning disable IDE0065

namespace Sudoku.Debugging
{
	using System;
	using System.Diagnostics;
	using System.Linq;
	using System.Text;
	using Sudoku.Diagnostics;
	using Sudoku.Extensions;
	using Sudoku.Solving.Annotations;
	using Sudoku.Solving.Manual;
	using static System.Console;

	/// <summary>
	/// The class aiming to this console application.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// The main function, which is the main entry point
		/// of this console application.
		/// </summary>
		private static void Main()
		{
			foreach (TechniqueCode? code in Enum.GetValues(typeof(TechniqueCode)))
			{
				var c = code!.Value;
				string? d = TechniqueDisplayAttribute.GetDisplayName(c);
				if (d is null)
				{
					var sb = new StringBuilder();
					string str = c.ToString();
					for (int i = 0; i < str.Length; i++)
					{
						char ch = str[i];
						if (char.IsUpper(ch) && i != 0)
						{
							sb.Append(" ");
						}

						sb.Append(ch);
					}

					d = sb.ToString();
				}
				Console.WriteLine($@"[""{c}""] = ""{d}"",");
			}

			//var w = new Stopwatch();

			//var z = new CodeCounter(Solution.PathRoot, @".+\.cs$");

			//w.Start();
			//int codeLines = z.CountCodeLines(out int count);
			//w.Stop();

			//foreach (var fileName in z.FileList)
			//{
			//	WriteLine(fileName);
			//}

			//WriteLine($"Code lines: {codeLines}, found files: {count}, time elapsed: {w.Elapsed:hh':'mm'.'ss'.'fff}");
		}
	}
}
