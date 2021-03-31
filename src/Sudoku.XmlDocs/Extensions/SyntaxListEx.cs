using System;
using Microsoft.CodeAnalysis;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SyntaxList{TNode}"/>.
	/// </summary>
	/// <seealso cref="SyntaxList{TNode}"/>
	public static class SyntaxListEx
	{
		/// <summary>
		/// Check whether all elements in the current list satisfied the specified condition.
		/// </summary>
		/// <typeparam name="TSyntaxNode">The type of the inner element.</typeparam>
		/// <param name="this">The list.</param>
		/// <param name="condition">The condition to check.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool All<TSyntaxNode>(
			this in SyntaxList<TSyntaxNode> @this, Predicate<TSyntaxNode> condition)
			where TSyntaxNode : SyntaxNode
		{
			foreach (var node in @this)
			{
				if (!condition(node))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Check whether the current list contains any syntax node that satisfied the specified condition.
		/// </summary>
		/// <typeparam name="TSyntaxNode">The type of the inner element.</typeparam>
		/// <param name="this">The list.</param>
		/// <param name="condition">The condition to check.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool Any<TSyntaxNode>(
			this in SyntaxList<TSyntaxNode> @this, Predicate<TSyntaxNode> condition)
			where TSyntaxNode : SyntaxNode
		{
			foreach (var node in @this)
			{
				if (condition(node))
				{
					return true;
				}
			}

			return false;
		}
	}
}
