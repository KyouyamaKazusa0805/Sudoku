using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SyntaxToken"/>.
	/// </summary>
	/// <seealso cref="SyntaxToken"/>
	public static class SyntaxTokenEx
	{
		/// <summary>
		/// Check whether the specified token is the keyword.
		/// </summary>
		/// <param name="this">The token.</param>
		/// <param name="keyword">The keyword.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsKeyword(this in SyntaxToken @this, string keyword) => @this.ValueText == keyword;

		/// <summary>
		/// Check whether the specified token is the specified keyword in the list <paramref name="keywords"/>.
		/// </summary>
		/// <param name="this">The token.</param>
		/// <param name="keywords">The keywords to check.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool IsKeyword(this in SyntaxToken @this, params string[] keywords)
		{
			foreach (string keyword in keywords)
			{
				if (@this.ValueText == keyword)
				{
					return true;
				}
			}

			return false;
		}
	}
}
