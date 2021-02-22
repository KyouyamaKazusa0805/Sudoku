using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.CodeAnalysis
{
	public sealed partial class DynamicallyInvocationOfCurrentAnalyzer
	{
		/// <summary>
		/// Indicates the searcher that stores the syntax nodes of the dynamically invocation of
		/// that <see langword="dynamic"/> field <c>Current</c>.
		/// </summary>
		private sealed class DynamicallyInvocationSearcher : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the semantic model.
			/// </summary>
			private readonly SemanticModel _semanticModel;


			/// <summary>
			/// Initializes an instance with the specified semantic model.
			/// </summary>
			/// <param name="semanticModel">The semantic model.</param>
			public DynamicallyInvocationSearcher(SemanticModel semanticModel) => _semanticModel = semanticModel;


			/// <summary>
			/// Indicates the valid collection of the <c>Current</c> dynamically invocation.
			/// </summary>
			public IList<(InvocationExpressionSyntax Node, string MethodName, ArgumentListSyntax ArgumentsNode)>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitInvocationExpression(InvocationExpressionSyntax node)
			{
				if
				(
					_semanticModel.GetOperation(node) is not IDynamicInvocationOperation
					{
						Kind: OperationKind.DynamicInvocation
					} operation
				)
				{
					return;
				}

				if
				(
					node is not
					{
						Expression: MemberAccessExpressionSyntax
						{
							RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
							Expression: MemberAccessExpressionSyntax
							{
								RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
								Expression: IdentifierNameSyntax
								{
									Identifier: { ValueText: TextResourcesClassName }
								},
								Name: IdentifierNameSyntax
								{
									Identifier: { ValueText: TextResourcesStaticReadOnlyFieldName }
								}
							},
							Name: IdentifierNameSyntax
							{
								Identifier: { ValueText: var methodName }
							}
						},
						ArgumentList: var argList
					}
				)
				{
					return;
				}

				Collection ??= new List<(InvocationExpressionSyntax, string, ArgumentListSyntax)>();

				Collection.Add((node, methodName, argList));
			}
		}
	}
}
