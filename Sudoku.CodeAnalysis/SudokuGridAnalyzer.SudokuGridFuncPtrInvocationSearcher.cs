using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
			/// Indicates the semantic model.
			/// </summary>
			private readonly SemanticModel _semanticModel;

			/// <summary>
			/// Indicates the compilation.
			/// </summary>
			private readonly Compilation _compilation;


			/// <summary>
			/// Initializes an instance with the specified semantic model.
			/// </summary>
			/// <param name="semanticModel">The semantic model.</param>
			/// <param name="compilation">The compilation.</param>
			public SudokuGridFuncPtrInvocationSearcher(SemanticModel semanticModel, Compilation compilation)
			{
				_semanticModel = semanticModel;
				_compilation = compilation;
			}


			/// <summary>
			/// Indicates the collection that stores all possible and valid information.
			/// </summary>
			public IList<(InvocationExpressionSyntax Node, string FieldName)>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitInvocationExpression(InvocationExpressionSyntax node)
			{
				if (_semanticModel.GetDeclaredSymbol(node) is not { Kind: SymbolKind.FunctionPointerType } symbol)
				{
					return;
				}

				if
				(
					node is not
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: MemberAccessExpressionSyntax
						{
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
					!SymbolEqualityComparer.Default.Equals(
						symbol.ContainingType,
						_compilation.GetTypeByMetadataName(SudokuGridFullTypeName)
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
