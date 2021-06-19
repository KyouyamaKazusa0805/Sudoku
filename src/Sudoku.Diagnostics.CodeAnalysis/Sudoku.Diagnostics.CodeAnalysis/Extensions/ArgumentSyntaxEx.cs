using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="ArgumentSyntax"/>.
	/// </summary>
	/// <seealso cref="ArgumentSyntax"/>
	public static class ArgumentSyntaxEx
	{
		/// <summary>
		/// Gets the full name of the parameter.
		/// </summary>
		/// <param name="this">The argument syntax node.</param>
		/// <param name="semanticModel">The semantic model.</param>
		/// <param name="cancellationToken">The cancellation token.</param>
		/// <returns>The full name.</returns>
		public static string? GetParamFullName(
			this ArgumentSyntax @this, SemanticModel semanticModel, CancellationToken cancellationToken)
		{
			var exprNode = @this.Expression;
			var operation = semanticModel.GetOperation(exprNode, cancellationToken);
			if (operation is null)
			{
				return null;
			}

			var typeSymbol = operation.Type;
			if (typeSymbol is null)
			{
				return null;
			}

			string typeStr = typeSymbol.ToDisplayString(NullableFlowState.None);
			return @this.RefKindKeyword.RawKind switch
			{
				(int)SyntaxKind.RefKeyword => $"ref {typeStr}",
				(int)SyntaxKind.OutKeyword => $"out {typeStr}",
				_ => typeStr
			};
		}
	}
}
