using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Sudoku.CodeAnalysis.Extensions;

namespace Sudoku.CodeAnalysis
{
	/// <summary>
	/// Indicates the analyzer that is called the resource dictionary values. Both two resource dictionaries
	/// store in the folder <c>..\required\lang</c>.
	/// </summary>
	[Generator]
	public sealed class ResourceDictionaryAnalyzer : ISourceGenerator
	{
		/// <summary>
		/// Indicates the text resources class name.
		/// </summary>
		private const string TextResourcesClassName = "TextResources";

		/// <summary>
		/// Indicates that field dynamically bound.
		/// </summary>
		private const string TextResourcesStaticReadOnlyFieldName = "Current";


		/// <inheritdoc/>
		public void Execute(GeneratorExecutionContext context)
		{
			var compilation = context.Compilation;
			if (compilation.AssemblyName is "Sudoku.UI" or "Sudoku.Windows")
			{
				// We don't check on those two WPF projects, because those two projects has already used
				// their own resource dictionary (MergedDictionary).
				return;
			}

			// Check all syntax trees if available.
			foreach (var syntaxTree in compilation.SyntaxTrees)
			{
				// Check whether the syntax contains the root node.
				if (!syntaxTree.TryGetRoot(out var root))
				{
					continue;
				}

				// Create the semantic model and the property list.
				var semanticModel = compilation.GetSemanticModel(syntaxTree);
				var collector = new DynamicallyUsingResourceDictionarySearcher(semanticModel);
				collector.Visit(root);

				// If the syntax tree doesn't contain any dynamically called clause,
				// just skip it.
				if (collector.Collection is null)
				{
					continue;
				}

				// Iterate on each dynamically called location.
				foreach (var (node, value) in collector.Collection)
				{
					var jsonProprtyNameRegex = new Regex($@"""{value}""(?=\:)");

					// Check all dictionaries. If all dictionaries don't contain that key,
					// we'll report on this.
					bool flag = false;
					foreach (var file in context.AdditionalFiles)
					{
						string json = File.ReadAllText(file.Path);
						var match = jsonProprtyNameRegex.Match(json);
						if (match.Success)
						{
							// Found that value. Just skip this case.
							flag = true;
						}
					}
					if (flag)
					{
						continue;
					}

					// Report the diagnostic result.
					context.ReportDiagnostic(
						Diagnostic.Create(
							new(
								id: DiagnosticIds.Sudoku008,
								title: Titles.Sudoku008,
								messageFormat: Messages.Sudoku008,
								category: Categories.ResourceDictionary,
								defaultSeverity: DiagnosticSeverity.Error,
								isEnabledByDefault: true,
								helpLinkUri: HelpLinks.Sudoku008
							),
							location: node.Name.GetLocation(),
							messageArgs: new[] { node.Name.Identifier.ValueText }
						)
					);
				}
			}
		}

		/// <inheritdoc/>
		public void Initialize(GeneratorInitializationContext context)
		{
		}


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
			public DynamicallyUsingResourceDictionarySearcher(SemanticModel semanticModel) => _semanticModel = semanticModel;


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
