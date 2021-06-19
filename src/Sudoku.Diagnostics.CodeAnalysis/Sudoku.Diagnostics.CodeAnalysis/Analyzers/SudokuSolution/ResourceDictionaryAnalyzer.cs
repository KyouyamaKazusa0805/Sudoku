using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGen;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0201")]
	public sealed partial class ResourceDictionaryAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterCompilationStartAction(static context =>
			{
				var (compilation, options) = context;

				// BUG: Can't load local dictionary files.
				/*length-pattern*/
				if (
					(from f in options.AdditionalFiles select File.ReadAllText(f.Path)).ToArray() is not
					{
						Length: not 0
					} texts
				)
				{
					// Check all files if available.
					return;
				}

				if (compilation.AssemblyName is "Sudoku.UI" or "Sudoku.Windows")
				{
					// We don't check on those two WPF projects, because those two projects has already used
					// their own resource dictionary (MergedDictionary).
					return;
				}

				context.RegisterSyntaxNodeAction(
					context => CheckWithUsingDirective(context, texts),
					new[] { SyntaxKind.SimpleMemberAccessExpression }
				);
			});
		}


		private static void CheckWithUsingDirective(SyntaxNodeAnalysisContext context, string[] texts)
		{
			var (semanticModel, _, node) = context;

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
						Expression: IdentifierNameSyntax { Identifier: { ValueText: "TextResources" } },
						Name: IdentifierNameSyntax { Identifier: { ValueText: "Current" } }
					},
					Name: IdentifierNameSyntax { Identifier: { ValueText: var key } } nameNode
				}
			)
			{
				return;
			}

			// Check all dictionaries.
			var jsonPropertyNameRegex = new Regex($@"""{key}""(?=\:\s*""[^""]+"",?)", RegexOptions.Compiled);
			if (texts.Any(text => jsonPropertyNameRegex.Match(text).Success))
			{
				// If all dictionaries don't contain that key,
				// we'll report on this.
				return;
			}

			// Report the diagnostic result.
			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0201,
					location: nameNode.GetLocation(),
					messageArgs: new[] { key }
				)
			);
		}
	}
}
