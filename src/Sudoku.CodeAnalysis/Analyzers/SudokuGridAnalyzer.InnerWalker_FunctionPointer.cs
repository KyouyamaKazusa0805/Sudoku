using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeAnalysis.Extensions;
using Pair = System.ValueTuple<Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax, string>;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class SudokuGridAnalyzer
	{
		partial void CheckSudoku014(GeneratorExecutionContext context, SyntaxNode root)
		{
			var collector = new InnerWalker_FunctionPointer();
			collector.Visit(root);

			// If the syntax tree doesn't contain any dynamically called clause,
			// just skip it.
			if (collector.Collection is null)
			{
				return;
			}

			// Iterate on each dynamically called location.
			foreach (var (node, fieldName) in collector.Collection)
			{
				// You can't invoke them.
				context.ReportDiagnostic(
					Diagnostic.Create(
						descriptor: new(
							id: DiagnosticIds.Sudoku014,
							title: Titles.Sudoku014,
							messageFormat: Messages.Sudoku014,
							category: Categories.Usage,
							defaultSeverity: DiagnosticSeverity.Error,
							isEnabledByDefault: true,
							helpLinkUri: HelpLinks.Sudoku014
						),
						location: node.GetLocation(),
						messageArgs: new[] { fieldName }
					)
				);
			}
		}

		/// <summary>
		/// The syntax walker that checks and gathers all invocation expression syntax nodes
		/// that uses the function pointer fields in <c>SudokuGrid</c>.
		/// </summary>
		private sealed class InnerWalker_FunctionPointer : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the type name of the sudoku grid.
			/// </summary>
			private const string SudokuGridTypeName = "SudokuGrid";

			/// <summary>
			/// Indicates the field name "<c>RefreshingCandidates</c>".
			/// </summary>
			private const string RefreshingCandidatesFuncPtrName = "RefreshingCandidates";

			/// <summary>
			/// Indicates the field name "<c>ValueChanged</c>".
			/// </summary>
			private const string ValueChangedFuncPtrName = "ValueChanged";


			/// <summary>
			/// Indicates the collection that stores all possible and valid information.
			/// </summary>
			public IList<Pair>? Collection { get; private set; }


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

				Collection ??= new List<Pair>();

				Collection.Add((node, fieldName));
			}
		}
	}
}
