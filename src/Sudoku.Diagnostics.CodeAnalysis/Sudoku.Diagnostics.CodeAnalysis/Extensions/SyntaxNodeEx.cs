using System;
using Microsoft.CodeAnalysis;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SyntaxNode"/>.
	/// </summary>
	/// <seealso cref="SyntaxNode"/>
	public static class SyntaxNodeEx
	{
		/// <summary>
		/// Check whether the containing type of the specified syntax node satisfies the
		/// specified condition.
		/// </summary>
		/// <param name="this">The syntax node.</param>
		/// <param name="predicate">
		/// The condition, where the parameter of the anonymous function or lambda is
		/// the node that used in traversing the iteration.
		/// </param>
		/// <returns>A <see cref="bool"/> result.</returns>
		public static bool ContainingTypeIs(this SyntaxNode @this, Predicate<SyntaxNode> predicate)
		{
			for (var currentNode = @this; currentNode is not null; currentNode = currentNode.Parent)
			{
				if (predicate(currentNode))
				{
					return true;
				}
			}

			return false;
		}
	}
}
