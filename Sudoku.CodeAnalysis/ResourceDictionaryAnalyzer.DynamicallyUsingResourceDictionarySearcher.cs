using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeAnalysis.Extensions;

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
				if (!node.IsKind(SyntaxKind.SimpleMemberAccessExpression))
				{
					return;
				}

				var exprNode = node.Expression;
				if (!exprNode.IsKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.IdentifierName))
				{
					return;
				}

				var operation = _semanticModel.GetOperation(node);
				if (operation is not { Kind: OperationKind.DynamicMemberReference })
				{
					return;
				}

				var exprOperation = _semanticModel.GetOperation(exprNode);
				if (exprOperation is not { Kind: OperationKind.FieldReference })
				{
					return;
				}

				if (exprNode.IsKind(SyntaxKind.SimpleMemberAccessExpression))
				{
					// TextResources.Current.XYZ
					// - SimpleMemberAccessExpression
					//   - Expression: SimpleMemberAccessExpression
					//     - Operation: FieldReference (TextResources.Current)
					//     - Expression: IdentifierName (TextResources)
					//     - Name: IdentifierName (Current)
					//   - Name: IdentifierName (XYZ)
					var memberAccessExprNode = (MemberAccessExpressionSyntax)exprNode;
					var parentExprNode = memberAccessExprNode.Expression;
					if (!parentExprNode.IsKind(SyntaxKind.IdentifierName))
					{
						return;
					}

					if (((IdentifierNameSyntax)parentExprNode).Identifier.ValueText != TextResourcesClassName)
					{
						return;
					}

					if (memberAccessExprNode.Name.Identifier.ValueText != TextResourcesStaticReadOnlyFieldName)
					{
						return;
					}

					Collection ??= new List<(MemberAccessExpressionSyntax, string)>();

					Collection.Add((node, node.Name.Identifier.ValueText));
				}
				else
				{
					// using static Sudoku.Resources.TextResources;
					// Current.XYZ
					// - SimpleMemberAccessExpression
					//   - Expression: IdentifierName (Current)
					//     - Operation: FieldReference (TextResources.Current)
					//   - Name: IdentifierName (XYZ)

					// TODO: Implement this case.
				}
			}
		}
	}
}
