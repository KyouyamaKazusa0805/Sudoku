using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Sudoku.DocComments;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SyntaxTrivia"/>.
	/// </summary>
	/// <seealso cref="SyntaxTrivia"/>
	public static class SyntaxTriviaEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">The syntax trivia.</param>
		/// <param name="kind">The syntax kind.</param>
		/// <param name="structure">The strutured syntax node (used for doc comments).</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(this in SyntaxTrivia @this, out SyntaxKind kind, out SyntaxNode? structure)
		{
			kind = (SyntaxKind)@this.RawKind;
			structure = @this.GetStructure();
		}
	}
}
