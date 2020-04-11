using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sudoku.Data;
using Sudoku.Extensions;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of node collection.
	/// </summary>
	[DebuggerStepThrough]
	public static class NodeCollection
	{
		/// <summary>
		/// Get a string consists of nodes' text.
		/// </summary>
		/// <param name="nodes">The nodes.</param>
		/// <returns>The string.</returns>
		public static string ToString(IEnumerable<Node> nodes)
		{
			const string separator = " -> ";

			bool @switch = false;
			var sb = new StringBuilder();
			foreach (var node in nodes)
			{
				sb.Append($"{(@switch ? string.Empty : "!")}{node}{separator}");
				@switch = !@switch;
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}
	}
}
