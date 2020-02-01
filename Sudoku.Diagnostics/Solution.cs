using System;
using System.IO;

namespace Sudoku.Diagnostics
{
	/// <summary>
	/// Provides the solution constant values.
	/// </summary>
	public static class Solution
	{
		/// <summary>
		/// The root path of this whole solution.
		/// </summary>
		public static readonly string PathRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.Parent.FullName;
	}
}
