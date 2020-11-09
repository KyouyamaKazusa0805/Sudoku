using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using Sudoku.Windows;

namespace Sudoku.CodeAnalysis
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class SudokuCodeAnalysisAnalyzer : DiagnosticAnalyzer
	{
		/// <summary>
		/// The rule.
		/// </summary>
		private static readonly DiagnosticDescriptor Rule = new(
			id: "SUDOKU001",
			title: "Missing value on resource dictionary",
			messageFormat: "Missing value on resource dictionary",
			category: "Naming",
			defaultSeverity: DiagnosticSeverity.Warning,
			isEnabledByDefault: true,
			description: "Missing value on resource dictionary.");

		/// <summary>
		/// All supported diagnostics.
		/// </summary>
		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);


		/// <inheritdoc/>
		public override void Initialize(AnalysisContext context)
		{
			context.EnableConcurrentExecution();
			context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);

			context.RegisterOperationAction(Analyze, OperationKind.Invocation);
		}

		/// <summary>
		/// To analyze the code.
		/// </summary>
		/// <param name="context">The context.</param>
		private void Analyze(OperationAnalysisContext context)
		{
			if (context.Operation is not IInvocationOperation operation)
			{
				return;
			}

			if (operation.TargetMethod?.Name is not "Sudoku.Windows.Resources.GetValue")
			{
				return;
			}

			var argumentsOfThisMethod = operation.Arguments;
			if (argumentsOfThisMethod is not { IsEmpty: false })
			{
				return;
			}

			var firstArg = argumentsOfThisMethod[0];
			if (firstArg?.Type.SpecialType is not SpecialType.System_String)
			{
				return;
			}

			var constantValue = firstArg.ConstantValue;
			if (!constantValue.HasValue || constantValue.Value is not string str)
			{
				return;
			}

			if (!new[] { Resources.LangSourceEnUs, Resources.LangSourceZhCn }.Any(dic => dic.ContainsKey(str)))
			{
				return;
			}

			context.ReportDiagnostic
			(
				Diagnostic.Create
				(
					Rule,
					operation.Syntax.GetLocation(),
					"Missing value on resource dictionary"
				)
			);
		}
	}
}
