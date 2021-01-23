using System;
using System.IO;
using Sudoku.Diagnostics;

namespace Sudoku.Debugging
{
	/// <summary>
	/// Provides the method to create a file counter, and count all files up.
	/// </summary>
	internal static class FileCounting
	{
		/// <summary>
		/// To count all files, and output the result using the <see cref="Console"/>.
		/// </summary>
		internal static void CountUp() =>
			Console.WriteLine
			(
				value: new FileCounter
				(
					root: Directory.GetParent(Environment.CurrentDirectory)!.Parent!.Parent!.Parent!.FullName,
					extension: "cs", 
					withBinOrObjDirectory: false
				).CountUp()
			);
	}
}
