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
			public IList<StructDeclarationSyntax> CandidateStructs { get; } = new List<StructDeclarationSyntax>();


			/// <inheritdoc/>
			public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
			{
				if (syntaxNode is StructDeclarationSyntax { AttributeLists: { Count: not 0 } } structDeclaration)
				{
					CandidateStructs.Add(structDeclaration);
				}
			}
		}
	}
}
