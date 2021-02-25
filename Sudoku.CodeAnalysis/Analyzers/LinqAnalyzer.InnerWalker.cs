using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeAnalysis.Extensions;
using Pair = System.ValueTuple<string, Microsoft.CodeAnalysis.CSharp.Syntax.BinaryExpressionSyntax>;

namespace Sudoku.CodeAnalysis.Analyzers
{
	partial class LinqAnalyzer
	{
		/// <summary>
		/// Indicates the inner walker.
		/// </summary>
		private class InnerWalker : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the method name <c>Take</c>.
			/// </summary>
			private const string TakeMethodName = "Take";

			/// <summary>
			/// Indicates the method name <c>Count</c>.
			/// </summary>
			private const string CountMethodName = "Count";

			/// <summary>
			/// Indicates the full type name of <see cref="Enumerable"/>.
			/// </summary>
			/// <seealso cref="Enumerable"/>
			private const string EnumerableClassFullName = "System.Linq.Enumerable";

			/// <summary>
			/// Indicates the full type name of <see cref="IEnumerable{T}"/>.
			/// </summary>
			/// <seealso cref="IEnumerable{T}"/>
			private const string IEnumerableFullName = "System.Collections.Generic.IEnumerable`1";


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
			public override void VisitBinaryExpression(BinaryExpressionSyntax node)
			{
				if
				(
					node is not
					{
						RawKind: (int)SyntaxKind.GreaterThanOrEqualExpression,
						Left: InvocationExpressionSyntax
						{
							Expression: MemberAccessExpressionSyntax
							{
								RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
								Expression:
									var potentialTakeMethodInvocationNode
									and not InvocationExpressionSyntax
									{
										Expression: MemberAccessExpressionSyntax
										{
											RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
											Expression: IdentifierNameSyntax
											{
												Identifier: { ValueText: TakeMethodName }
											}
										}
									},
								Name: IdentifierNameSyntax { Identifier: { ValueText: CountMethodName } },
							},
							ArgumentList: { Arguments: { Count: 0 } }
						} invocationNode,
						Right: var rightNode
					}
				)
				{
					return;
				}

				if
				(
					_semanticModel.GetOperation(invocationNode) is not IInvocationOperation
					{
						Kind: OperationKind.Invocation,
						TargetMethod:
						{
							ContainingType: var containingTypeSymbol,
							Parameters: { Length: 1 } parameterSymbols,
							ReturnType: var returnTypeSymbol,
							IsExtensionMethod: true,
							IsGenericMethod: true
						}
					} operation
				)
				{
					return;
				}

				if
				(
					_semanticModel.GetOperation(potentialTakeMethodInvocationNode) is IInvocationOperation
					{
						Kind: OperationKind.Invocation,
						TargetMethod:
						{
							ContainingType: var containingTypeSymbolTakeMethod,
							Parameters: { Length: 2 } parameterSymbolsTakeMethod,
							ReturnType: var returnTypeSymbolTakeMethod,
							IsExtensionMethod: true,
							IsGenericMethod: true
						}
					} possibleTakeMethodInvocationOperation
					&& SymbolEqualityComparer.Default.Equals(
						containingTypeSymbolTakeMethod,
						_compilation.GetTypeByMetadataName(EnumerableClassFullName)
					)
					&& SymbolEqualityComparer.Default.Equals(
						returnTypeSymbolTakeMethod,
						_compilation
						.GetTypeByMetadataName(IEnumerableFullName)!
						.WithTypeArguments(_compilation, SpecialType.System_Int32)
					)
					&& SymbolEqualityComparer.Default.Equals(
						parameterSymbolsTakeMethod[0].Type,
						_compilation
						.GetTypeByMetadataName(IEnumerableFullName)!
						.WithTypeArguments(_compilation, SpecialType.System_Int32)
					)
					&& SymbolEqualityComparer.Default.Equals(
						parameterSymbolsTakeMethod[1].Type,
						_compilation.GetSpecialType(SpecialType.System_Int32)
					)
				)
				{
					return;
				}

				if
				(
					!SymbolEqualityComparer.Default.Equals(
						containingTypeSymbol,
						_compilation.GetTypeByMetadataName(EnumerableClassFullName)
					)
					|| !SymbolEqualityComparer.Default.Equals(
						returnTypeSymbol,
						_compilation.GetSpecialType(SpecialType.System_Int32)
					)
					|| !SymbolEqualityComparer.Default.Equals(
						parameterSymbols[0].Type,
						_compilation
						.GetTypeByMetadataName(IEnumerableFullName)!
						.WithTypeArguments(_compilation, SpecialType.System_Int32)
					)
				)
				{
					return;
				}

				Collection ??= new List<Pair>();

				Collection.Add((rightNode.ToString(), node));
			}
		}
	}
}
