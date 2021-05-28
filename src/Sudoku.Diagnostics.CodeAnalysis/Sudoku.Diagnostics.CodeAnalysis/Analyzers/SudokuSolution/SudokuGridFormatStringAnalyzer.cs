using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Diagnostics.Extensions;

namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers
{
	/// <summary>
	/// Indicates an analyzer that analyzes the code for the method invocation <c>ToString</c>
	/// of type <c>SudokuGrid</c>.
	/// </summary>
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public sealed partial class SudokuGridFormatStringAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// Indicates the full type name of the sudoku grid.
		/// </summary>
		private const string SudokuGridFullTypeName = "Sudoku.Data.SudokuGrid";


		/// <summary>
		/// All possible format strings to check.
		/// </summary>
		private static readonly string[] PossibleFormats = new[]
		{
			".", "+", ".+", "+.", "0", ":", "!", ".!", "!.", "0!", "!0",
			".:", "0:", "0+", "+0", "+:", "+.:", ".+:", "#", "#.", "0+:",
			"+0:", "#0", ".!:", "!.:", "0!:", "!0:", "@", "@.", "@0", "@!",
			"@.!", "@!.", "@0!", "@!0", "@*", "@*.", "@.*", "@0*", "@*0",
			"@!*", "@*!", "@:", "@:!", "@!:", "@*:", "@:*", "@!*:", "@*!:",
			"@!:*", "@:!*", "@:!*", "@:*!", "~", "~0", "~.", "@~", "~@", "@~0",
			"@0~", "~@0", "~0@", "@~.", "@.~", "~@.", "~.@", "%", "^"
		};


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
			context.EnableConcurrentExecution();

			context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.InvocationExpression });
		}


		private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
		{
			var (semanticModel, compilation, originalNode) = context;

			/*length-pattern*/
			if (
				originalNode is not InvocationExpressionSyntax
				{
					Expression: MemberAccessExpressionSyntax
					{
						RawKind: (int)SyntaxKind.SimpleMemberAccessExpression,
						Expression: var expression,
						Name: { Identifier: { ValueText: "ToString" } } nameNode
					},
					ArgumentList: { Arguments: { Count: var count } arguments }
				} node
			)
			{
				return;
			}

			if (count == 0)
			{
				CheckSD0310(context, semanticModel, compilation, expression, nameNode);
			}
			else if (
				/*slice-pattern*/
				arguments[0] is { Expression: var expr } argument
				&& semanticModel.GetOperation(expr) is { ConstantValue: { HasValue: true, Value: string value } }
			)
			{
				CheckSD0311(context, argument, value);
			}
		}

		private static void CheckSD0310(
			SyntaxNodeAnalysisContext context, SemanticModel semanticModel, Compilation compilation,
			ExpressionSyntax expression, SimpleNameSyntax nameNode)
		{
			if (semanticModel.GetOperation(expression) is not { Type: var possibleSudokuGridType })
			{
				return;
			}

			var sudokuGridType = compilation.GetTypeByMetadataName(SudokuGridFullTypeName);
			if (!SymbolEqualityComparer.Default.Equals(possibleSudokuGridType, sudokuGridType))
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0310,
					location: nameNode.GetLocation(),
					messageArgs: null
				)
			);
		}

		private static void CheckSD0311(
			SyntaxNodeAnalysisContext context, ArgumentSyntax argument, string format)
		{
			if (Array.IndexOf(PossibleFormats, format) != -1)
			{
				return;
			}

			context.ReportDiagnostic(
				Diagnostic.Create(
					descriptor: SD0311,
					location: argument.GetLocation(),
					messageArgs: null
				)
			);
		}
	}
}
