namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SS0102F")]
public sealed partial class InterpolatedStringAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(
			AnalyzeSyntaxNode,
			new[] { SyntaxKind.Interpolation, SyntaxKind.InterpolatedStringExpression }
		);
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		if (context.Node is not InterpolatedStringExpressionSyntax node)
		{
			return;
		}

		if (node.DescendantNodes().OfType<InterpolationSyntax>().Any())
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SS0102,
				location: node.GetLocation(),
				messageArgs: null
			)
		);
	}
}
