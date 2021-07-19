using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.CodeGenerating;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	[CodeAnalyzer("SD0315F")]
	public sealed partial class ValueStringBuilderUsingDeclarationAnalyzer : DiagnosticAnalyzer
	{
		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[] { SyntaxKind.LocalDeclarationStatement, SyntaxKind.UsingStatement }
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode, _, cancellationToken) = context;

			var vsbType = compilation.GetTypeByMetadataName("System.Text.ValueStringBuilder");
			switch (originalNode)
			{
				// using var ...;
				case LocalDeclarationStatementSyntax
				{
					UsingKeyword: { RawKind: (int)SyntaxKind.UsingKeyword } usingKeyword,
					Declaration: var declaration
				}
				when whenClause(declaration):
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0315,
							location: usingKeyword.GetLocation(),
							messageArgs: null
						)
					);

					break;
				}

				// using (...)
				// {
				//     ...
				// }
				case UsingStatementSyntax { UsingKeyword: var usingKeyword, Declaration: { } declaration }
				when whenClause(declaration):
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0315,
							location: usingKeyword.GetLocation(),
							messageArgs: null
						)
					);

					break;
				}


				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				bool whenClause(VariableDeclarationSyntax declaration) =>
					semanticModel.GetOperation(declaration, cancellationToken) is IVariableDeclarationOperation
					{
						Declarators: { Length: not 0 } declarators
					} && SymbolEqualityComparer.Default.Equals(vsbType, declarators[0].Symbol.Type);
			}
		}
	}
}
