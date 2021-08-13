namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0633")]
public sealed partial class UseDefaultCaseAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			AnalyzeSyntaxNode,
			new[]
			{
				SyntaxKind.CasePatternSwitchLabel,
				SyntaxKind.SwitchExpressionArm
			}
		);
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		if (
			context.Node switch
			{
				CasePatternSwitchLabelSyntax
				{
					Pattern: VarPatternSyntax { Designation: DiscardDesignationSyntax },
					WhenClause: null
				} node => Diagnostic.Create(
					descriptor: SS0633,
					location: node.GetLocation(),
					properties: ImmutableDictionary.CreateRange(
						new KeyValuePair<string, string?>[] { new("IsSwitchExpression", "False") }
					),
					messageArgs: null
				),
				SwitchExpressionArmSyntax
				{
					Pattern: VarPatternSyntax { Designation: DiscardDesignationSyntax } pattern,
					WhenClause: null
				} node => Diagnostic.Create(
					descriptor: SS0633,
					location: pattern.GetLocation(),
					properties: ImmutableDictionary.CreateRange(
						new KeyValuePair<string, string?>[] { new("IsSwitchExpression", "True") }
					),
					messageArgs: null
				),
				_ => null
			} is { } resultDiagnostic
		)
		{
			context.ReportDiagnostic(resultDiagnostic);
		}
	}
}
