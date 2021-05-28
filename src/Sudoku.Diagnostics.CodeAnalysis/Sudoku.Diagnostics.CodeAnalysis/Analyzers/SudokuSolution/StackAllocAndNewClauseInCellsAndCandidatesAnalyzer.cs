using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;
using Microsoft.CodeAnalysis.Operations;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates the analyzer that analyzes on types <c>Cells</c> and <c>Candidates</c>,
	/// to check whether the input value in the initializer is invalid.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class StackAllocAndNewClauseInCellsAndCandidatesAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the full type name of <c>Cells</c>.
		/// </summary>
		private const string CellsFullTypeName = "Sudoku.Data.Cells";

		/// <summary>
		/// Indicates the full type name of <c>Candidates</c>.
		/// </summary>
		private const string CandidatesFullTypeName = "Sudoku.Data.Candidates";


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(
				AnalyzeSyntaxNode,
				new[]
				{
					SyntaxKind.ObjectCreationExpression,
					SyntaxKind.ImplicitObjectCreationExpression,
					SyntaxKind.StackAllocArrayCreationExpression,
					SyntaxKind.ImplicitStackAllocArrayCreationExpression
				}
			);
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode) = context;

			/*length-pattern*/
			if (
				originalNode is not BaseObjectCreationExpressionSyntax
				{
					ArgumentList: { Arguments: { Count: 1 } arguments }
				} node
			)
			{
				return;
			}

			if (
				semanticModel.GetOperation(node) is not IObjectCreationOperation
				{
					Kind: OperationKind.ObjectCreation,
					Type: var typeSymbol
				}
			)
			{
				return;
			}

			bool isOfTypeCells = SymbolEqualityComparer.Default.Equals(
				typeSymbol,
				compilation.GetTypeByMetadataName(CellsFullTypeName)
			);
			bool isOfTypeCandidates = SymbolEqualityComparer.Default.Equals(
				typeSymbol,
				compilation.GetTypeByMetadataName(CandidatesFullTypeName)
			);
			if (!isOfTypeCells && !isOfTypeCandidates)
			{
				return;
			}

			switch (arguments[0].Expression)
			{
				case ArrayCreationExpressionSyntax newExpression:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0309,
							location: newExpression.GetLocation(),
							messageArgs: new[] { "new" }
						)
					);

					break;
				}
				case ImplicitArrayCreationExpressionSyntax implicitNewExpression:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0309,
							location: implicitNewExpression.GetLocation(),
							messageArgs: new[] { "new" }
						)
					);

					break;
				}
				case StackAllocArrayCreationExpressionSyntax stackAllocExpression:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0309,
							location: stackAllocExpression.GetLocation(),
							messageArgs: new[] { "stackalloc" }
						)
					);

					break;
				}
				case ImplicitStackAllocArrayCreationExpressionSyntax implicitStackAllocExpression:
				{
					context.ReportDiagnostic(
						Diagnostic.Create(
							descriptor: SD0309,
							location: implicitStackAllocExpression.GetLocation(),
							messageArgs: new[] { "stackalloc" }
						)
					);

					break;
				}
			}
		}
	}
}
