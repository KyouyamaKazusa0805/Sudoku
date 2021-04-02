using System;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="XmlElementEndTagSyntax"/>.
	/// </summary>
	/// <seealso cref="XmlElementEndTagSyntax"/>
	public static class XmlElementEndTagSyntaxEx
	{
		/// <summary>
		/// Check whether the current tag is the specified markup.
		/// </summary>
		/// <param name="this">The node to check.</param>
		/// <param name="textToCheck">The markup text to check.</param>
		/// <returns>A <see cref="bool"/> result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsMarkup(this XmlElementEndTagSyntax @this, string textToCheck) =>
			@this.Name.LocalName.ValueText.Equals(textToCheck, StringComparison.OrdinalIgnoreCase);
	}
}
