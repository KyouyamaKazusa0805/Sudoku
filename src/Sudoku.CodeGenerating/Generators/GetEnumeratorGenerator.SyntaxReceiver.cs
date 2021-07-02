using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeGenerating
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
								attribute.Name is IdentifierNameSyntax identifierName
								&& identifierName.Identifier.ValueText is var t
								&& nameof(AutoGetEnumeratorAttribute) is var attributeName
								&& (
									t == attributeName
									|| t == attributeName.Substring(0, attributeName.Length - 9)
								)
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
