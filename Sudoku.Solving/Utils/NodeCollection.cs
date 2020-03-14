using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Solving.Manual.Chaining;

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
		public static string ToString(IEnumerable<ChainNode> nodes)
		{
			const string separator = " -> ";

			var sb = new StringBuilder();
			foreach (var node in nodes)
			{
				sb.Append($"{node}{separator}");
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}

		/// <summary>
		/// Get a string consists of nodes' text.
		/// </summary>
		/// <param name="nodes">The nodes.</param>
		/// <returns>The string.</returns>
		public static string ToString(IEnumerable<Node> nodes)
		{
			const string separator = " -> ";

			var sb = new StringBuilder();
			foreach (var node in nodes)
			{
				sb.Append($"{node}{separator}");
			}

			return sb.RemoveFromEnd(separator.Length).ToString();
		}
	}
}
