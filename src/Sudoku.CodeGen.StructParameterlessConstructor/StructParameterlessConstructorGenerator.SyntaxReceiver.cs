using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeGen.StructParameterlessConstructor
{
	partial class StructParameterlessConstructorGenerator
	{
		/// <summary>
		/// Defines the syntax receiver.
		/// </summary>
		private sealed class SyntaxReceiver : ISyntaxReceiver
		{
			/// <summary>
			/// Indicates all possible candidate <see langword="class"/>es used.
			/// </summary>
			public IList<ClassDeclarationSyntax> CandidateStructs { get; } = new List<ClassDeclarationSyntax>();


			/// <inheritdoc/>
			public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
			{
				// Any field with at least one attribute is a candidate for property generation.
				if (syntaxNode is ClassDeclarationSyntax { AttributeLists: { Count: not 0 } } classDeclaration)
				{
					CandidateStructs.Add(classDeclaration);
				}
			}
		}
	}
}
