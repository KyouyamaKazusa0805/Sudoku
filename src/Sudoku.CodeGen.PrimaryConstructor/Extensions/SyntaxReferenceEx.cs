using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Sudoku.CodeGen.PrimaryConstructor.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="SyntaxReference"/>.
	/// </summary>
	/// <seealso cref="SyntaxReference"/>
	public static class SyntaxReferenceEx
	{
		/// <summary>
		/// Deconstruct the <see cref="SyntaxReference"/> instance to the current <see cref="TextSpan"/>
		/// and the <see cref="SyntaxNode"/>.
		/// </summary>
		/// <param name="this">The current syntax reference instance.</param>
		/// <param name="textSpan">The text span.</param>
		/// <param name="syntaxNode">The syntax node.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Deconstruct(
			this SyntaxReference @this, out TextSpan textSpan, out SyntaxNode syntaxNode)
		{
			textSpan = @this.Span;
			syntaxNode = @this.GetSyntax();
		}
	}
}
