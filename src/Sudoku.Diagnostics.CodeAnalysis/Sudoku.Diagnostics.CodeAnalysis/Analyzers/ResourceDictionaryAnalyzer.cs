using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that is called the resource dictionary values. Both two resource dictionaries
	/// store in the folder <c>..\required\lang</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class ResourceDictionaryAnalyzer : DiagnosticAnalyzer
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
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(CheckSD0201, new[] { SyntaxKind.SimpleMemberAccessExpression });
		}


		private static void CheckSD0201(SyntaxNodeAnalysisContext context)
		{
			if (
				(from file in context.Options.AdditionalFiles select File.ReadAllText(file.Path)).ToArray()
				is not { Length: not 0 } texts
			)
			{
				// Check all syntax trees if available.
				return;
			}

			var (semanticModel, compilation, node) = context;
			if (compilation.AssemblyName is "Sudoku.UI" or "Sudoku.Windows")
			{
				// We don't check on those two WPF projects, because those two projects has already used
				// their own resource dictionary (MergedDictionary).
				return;
			}

			if (semanticModel.GetOperation(node) is not { Kind: OperationKind.DynamicMemberReference })
			{
				return;
			}

			if (
				node is not MemberAccessExpressionSyntax
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
					} nameNode
				}
			)
			{
				return;
			}

			// Check all dictionaries.
			var jsonPropertyNameRegex = new Regex($@"""{key}""(?=\:\s""[^""]+"",?)", RegexOptions.Compiled);
			if (texts.Any(text => jsonPropertyNameRegex.Match(text).Success))
			{
				// If all dictionaries don't contain that key,
				// we'll report on this.
				return;
			}

			// Report the diagnostic result.
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: new(
						id: DiagnosticIds.SD0201,
						title: Titles.SD0201,
						messageFormat: Messages.SD0201,
						category: Categories.ResourceDictionary,
						defaultSeverity: DiagnosticSeverity.Error,
						isEnabledByDefault: true,
						helpLinkUri: HelpLinks.SD0201
					),
					location: nameNode.GetLocation(),
					messageArgs: new[] { key }
				)
			);

			// TODO: Implement another case that is with a using static directive.
		}
	}
}
