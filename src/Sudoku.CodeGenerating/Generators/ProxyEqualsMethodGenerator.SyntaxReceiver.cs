using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeGenerating
{
	partial class ProxyEqualsMethodGenerator
	{
		/// <summary>
		/// Indicates the inner syntax receiver.
		/// </summary>
		private sealed class SyntaxReceiver : ISyntaxReceiver
		{
			/// <summary>
			/// Indicates the result types.
			/// </summary>
			public IList<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();


			/// <inheritdoc/>
			public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
			{
				if (syntaxNode is TypeDeclarationSyntax type and not InterfaceDeclarationSyntax)
				{
					Candidates.Add(type);
				}
			}
		}
	}
}
