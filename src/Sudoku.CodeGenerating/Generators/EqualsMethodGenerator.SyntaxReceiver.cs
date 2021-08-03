using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeGenerating.Generators
{
	partial class EqualsMethodGenerator
	{
		/// <summary>
		/// Defines the syntax receiver.
		/// </summary>
		private sealed class SyntaxReceiver : ISyntaxReceiver
		{
			/// <summary>
			/// Indicates all possible candidate types used.
			/// </summary>
			public IList<TypeDeclarationSyntax> Candidates { get; } = new List<TypeDeclarationSyntax>();


			/// <inheritdoc/>
			public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
			{
				// Any field with at least one attribute is a candidate for property generation.
				if (syntaxNode is TypeDeclarationSyntax { AttributeLists: { Count: not 0 } } classDeclaration)
				{
					Candidates.Add(classDeclaration);
				}
			}
		}
	}
}
