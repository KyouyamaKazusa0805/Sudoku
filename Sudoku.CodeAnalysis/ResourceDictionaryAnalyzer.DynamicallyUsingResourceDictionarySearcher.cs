using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sudoku.CodeAnalysis
{
	partial class ResourceDictionaryAnalyzer
	{
		/// <summary>
		/// Indicates the syntax walker that searches and visits the syntax node that is:
		/// <list type="bullet">
		/// <item><c>TextResources.Current.KeyToGet</c></item>
		/// <item>
		/// <c>Current.KeyToGet</c> (need the directive <c>using static Sudoku.Resources.TextResources;</c>)
		/// </item>
		/// </list>
		/// </summary>
		/// <remarks>
		/// Please note that in this case the analyzer won't check: <c>Current["KeyToGet"]</c>, because
		/// this case allows the parameter is a local variable, which isn't a constant.
		/// </remarks>
		private sealed class DynamicallyUsingResourceDictionarySearcher : CSharpSyntaxWalker
		{
			/// <summary>
			/// Indicates the semantic model of this syntax tree.
			/// </summary>
			private readonly SemanticModel _semanticModel;


			/// <summary>
			/// Initializes an instance with the specified semantic model.
			/// </summary>
			/// <param name="semanticModel">The semantic model.</param>
			public DynamicallyUsingResourceDictionarySearcher(SemanticModel semanticModel) =>
				_semanticModel = semanticModel;


			/// <summary>
			/// Indicates the collection that stores those nodes.
			/// </summary>
			public IList<(MemberAccessExpressionSyntax Node, string Value)>? Collection { get; private set; }


			/// <inheritdoc/>
			public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
			{
				if (_semanticModel.GetOperation(node) is not { Kind: OperationKind.DynamicMemberReference })
				{
					return;
				}

				if
				(
					node is not
					{
						Parent: not InvocationExpressionSyntax,
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
							Identifier: { ValueText: var key }
						}
					}
				)
				{
					return;
				}

				Collection ??= new List<(MemberAccessExpressionSyntax, string)>();

				Collection.Add((node, key));

				// TODO: Implement another case that is with a using static directive.
			}
		}
	}
}
