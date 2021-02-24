using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Quadruple = System.ValueTuple<
	Microsoft.CodeAnalysis.CSharp.Syntax.InvocationExpressionSyntax,
	Microsoft.CodeAnalysis.CSharp.Syntax.IdentifierNameSyntax,
	string,
	Microsoft.CodeAnalysis.CSharp.Syntax.ArgumentListSyntax
>;

namespace Sudoku.CodeAnalysis
{
	partial class DynamicallyInvocationOfCurrentAnalyzer
	{
		/// <summary>
		/// Indicates the searcher that stores the syntax nodes of the dynamically invocation of
		/// that <see langword="dynamic"/> field <c>Current</c>.
		/// </summary>
		private sealed class InnerWalker : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the semantic model.
			/// </summary>
			private readonly SemanticModel _semanticModel;


			/// <summary>
			/// Initializes an instance with the specified semantic model.
			/// </summary>
			/// <param name="semanticModel">The semantic model.</param>
			public InnerWalker(SemanticModel semanticModel) => _semanticModel = semanticModel;


			/// <summary>
			/// Indicates the valid collection of the <c>Current</c> dynamically invocation.
			/// </summary>
			public IList<Quadruple>? Collection { get; private set; }


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
							} nameNode
						},
						ArgumentList: var argList
					}
				)
				{
					return;
				}

				Collection ??= new List<Quadruple>();

				Collection.Add((node, nameNode, methodName, argList));
			}
		}
	}
}
