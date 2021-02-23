using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeAnalysis.Extensions;

namespace Sudoku.CodeAnalysis
{
	partial class SudokuGridAnalyzer
	{
		/// <summary>
		/// The syntax walker that checks and gathers all invocation expression syntax nodes
		/// that uses the function pointer fields in <c>SudokuGrid</c>.
		/// </summary>
		private sealed class SudokuGridFuncPtrInvocationSearcher : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the collection that stores all possible and valid information.
			/// </summary>
			public IList<(InvocationExpressionSyntax Node, string FieldName)>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitInvocationExpression(InvocationExpressionSyntax node)
			{
				if
				(
					node is not
					{
						Expression: MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
							Expression: IdentifierNameSyntax { Identifier: { ValueText: SudokuGridTypeName } },
							Name: IdentifierNameSyntax
							{
								Identifier:
								{
									ValueText: var fieldName and (
										ValueChangedFuncPtrName or RefreshingCandidatesFuncPtrName
									)
								}
							}
						}
					}
				)
				{
					return;
				}

				if
				(
					node.ContainingTypeIs(
						static nodeTraversing => nodeTraversing is StructDeclarationSyntax
						{
							Identifier: { ValueText: SudokuGridTypeName }
						}
					)
				)
				{
					return;
				}

				Collection ??= new List<(InvocationExpressionSyntax, string)>();

				Collection.Add((node, fieldName));
			}
		}
	}
}
