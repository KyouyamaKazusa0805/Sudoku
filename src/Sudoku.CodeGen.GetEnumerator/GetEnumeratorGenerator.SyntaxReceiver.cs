using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeGen.GetEnumerator
{
	partial class GetEnumeratorGenerator
	{
		/// <summary>
		/// Indicates the inner syntax receiver.
		/// </summary>
		private sealed class SyntaxReceiver : ISyntaxReceiver
		{
			/// <summary>
			/// Indicates the types that satisfy the condition.
			/// </summary>
			public IList<(TypeDeclarationSyntax Node, AttributeSyntax Attribute)> Candidates { get; } =
				new List<(TypeDeclarationSyntax, AttributeSyntax)>();


			/// <inheritdoc/>
			public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
			{
				if (
					syntaxNode is TypeDeclarationSyntax
					{
						AttributeLists: { Count: not 0 } attributeLists
					} declaration
				)
				{
					foreach (var attributeList in attributeLists)
					{
						foreach (var attribute in attributeList.Attributes)
						{
							if (
								attribute is
								{
									Name: IdentifierNameSyntax
									{
										Identifier: { ValueText: nameof(AutoGetEnumeratorAttribute) }
									}
								}
							)
							{
								Candidates.Add((declaration, attribute));
								return;
							}
						}
					}
				}
			}
		}
	}
}
