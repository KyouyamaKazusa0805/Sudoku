using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeAnalysis.Extensions;
using Pair = System.ValueTuple<bool, Microsoft.CodeAnalysis.CSharp.Syntax.BaseObjectCreationExpressionSyntax>;

namespace Sudoku.CodeAnalysis
{
	partial class SpanOrReadOnlySpanAnalyzer
	{
		/// <summary>
		/// Indicates the inner walker to check implicit or explicit <see langword="new"/> clause.
		/// </summary>
		private sealed class InnerWalker : CSharpSyntaxWalker
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
			/// Initializes an instance with the specified semantic model and the compilation.
			/// </summary>
			/// <param name="semanticModel">The semantic model.</param>
			/// <param name="compilation">The compilation.</param>
			public InnerWalker(SemanticModel semanticModel, Compilation compilation)
			{
				_semanticModel = semanticModel;
				_compilation = compilation;
			}


			/// <summary>
			/// Indicates the result collection.
			/// </summary>
			public IList<Pair>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
			{
				if (node.Body is { } body)
				{
					InternalVisit(body);
				}
			}

			/// <inheritdoc/>
			public override void VisitGlobalStatement(GlobalStatementSyntax node) => InternalVisit(node);

			/// <inheritdoc/>
			public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
			{
				if (node.Body is { } body)
				{
					InternalVisit(body);
				}
			}

			/// <summary>
			/// Visit methods.
			/// </summary>
			/// <param name="node">The root node of the function.</param>
			private void InternalVisit(SyntaxNode node)
			{
				var descendants = node.DescendantNodes().ToArray();
				foreach (var descendant in descendants)
				{
					// Check whether the block contains explicit or implicit new clauses.
					if
					(
						descendant is not BaseObjectCreationExpressionSyntax
						{
							RawKind:
								(int)SyntaxKind.ObjectCreationExpression
								or (int)SyntaxKind.ImplicitObjectCreationExpression
						} newClauseNode
					)
					{
						continue;
					}

					// Check operation and get the constructor symbol.
					var operation = _semanticModel.GetOperation(newClauseNode);
					if
					(
						operation is not IObjectCreationOperation
						{
							Kind: OperationKind.ObjectCreation,
							Constructor: { } constructorSymbol
						}
					)
					{
						continue;
					}

					// Check type.
					INamedTypeSymbol
						typeSymbol = constructorSymbol.ContainingType.ConstructUnboundGenericType(),
						spanTypeSymbol = _compilation
							.GetTypeByMetadataName(SpanTypeFullName)!
							.ConstructUnboundGenericType(),
						readOnlySpanTypeSymbol = _compilation
							.GetTypeByMetadataName(ReadOnlySpanTypeFullName)!
							.ConstructUnboundGenericType();
					if (!SymbolEqualityComparer.Default.Equals(typeSymbol, spanTypeSymbol)
						&& !SymbolEqualityComparer.Default.Equals(typeSymbol, readOnlySpanTypeSymbol))
					{
						continue;
					}

					// Check parameters.
					var @params = constructorSymbol.Parameters;
					if (@params.Length != 2)
					{
						continue;
					}

					if
					(
						!SymbolEqualityComparer.Default.Equals(
							@params[0].Type,
							_compilation.GetPointerTypeSymbol(SpecialType.System_Void)
						)
						|| !SymbolEqualityComparer.Default.Equals(
							@params[1].Type,
							_compilation.GetSpecialType(SpecialType.System_Int32)
						)
					)
					{
						continue;
					}

					// Potential syntax node found.
					// If the first argument is a variable, check the assignment.
					if
					(
						newClauseNode.ArgumentList?.Arguments[0] is not
						{
							RawKind: (int)SyntaxKind.Argument,
							Expression: IdentifierNameSyntax
							{
								Identifier: { ValueText: var variableName }
							}
						}
					)
					{
						continue;
					}

					// Check the local variable.
					// If the assignment is 'stackalloc' clause, we can report the diagnostic result.
					if
					(
						Array.Find(
							descendants,
							element => element is VariableDeclaratorSyntax
							{
								Identifier: { ValueText: var localVariableName }
							} && localVariableName == variableName
						) is not VariableDeclaratorSyntax
						{
							Initializer: { Value: StackAllocArrayCreationExpressionSyntax }
						}
					)
					{
						continue;
					}

					Collection ??= new List<Pair>();

					Collection.Add(
						(SymbolEqualityComparer.Default.Equals(typeSymbol, spanTypeSymbol), newClauseNode)
					);
				}
			}
		}
	}
}
