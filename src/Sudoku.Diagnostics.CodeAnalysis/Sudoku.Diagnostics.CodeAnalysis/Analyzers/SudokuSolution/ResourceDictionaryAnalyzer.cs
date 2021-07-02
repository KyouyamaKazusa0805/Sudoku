using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Sudoku.CodeGenerating;

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
				if (compilation.AssemblyName is ProjectNames.Sudoku_Windows or ProjectNames.Sudoku_UI)
				{
					// We don't check on those two WPF projects, because those two projects has already used
					// their own resource dictionary (MergedDictionary).
					return;
				}

				context.RegisterSyntaxNodeAction(
					CheckWithUsingDirective,
					new[] { SyntaxKind.SimpleMemberAccessExpression }
				);
			});
		}


		private static void CheckWithUsingDirective(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, _, originalNode) = context;

			if (semanticModel.GetOperation(originalNode) is not { Kind: OperationKind.DynamicMemberReference })
			{
				return;
			}

			if (
				originalNode is not MemberAccessExpressionSyntax
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

			// Check all keys.
			if (ResourceDictionaryKeys.Keys.Contains(key))
			{
				return;
			}

			// If all dictionaries don't contain that key, we'll report on this.
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
