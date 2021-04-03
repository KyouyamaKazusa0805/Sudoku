using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.DocComments;

namespace Sudoku.XmlDocs.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ParameterSyntax"/>.
	/// </summary>
	/// <seealso cref="ParameterSyntax"/>
	public static class ParameterSyntaxEx
	{
		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">The current parameter syntax node.</param>
		/// <param name="type">The type syntax node.</param>
		/// <param name="identifier">The identifier syntax token.</param>
		public static void Deconstruct(this ParameterSyntax @this, out TypeSyntax? type, out SyntaxToken identifier)
		{
			type = @this.Type;
			identifier = @this.Identifier;
		}
	}
}
