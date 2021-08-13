namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[CodeAnalyzer("SD0502")]
public sealed partial class ExtensionMethodParameterNamingAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc/>
	public override void Initialize(AnalysisContext context)
	{
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();

		context.RegisterSyntaxNodeAction(AnalyzeSyntaxNode, new[] { SyntaxKind.MethodDeclaration });
	}


	private static void AnalyzeSyntaxNode(SyntaxNodeAnalysisContext context)
	{
		if (
			context.Node is not MethodDeclarationSyntax
			{
				Identifier: { ValueText: var name } identifier,
				Modifiers: { Count: not 0 } modifiers,
				ParameterList.Parameters: { Count: not 0 } parameters
			}
		)
		{
			return;
		}

		if (modifiers.All(static modifier => modifier.RawKind != (int)SyntaxKind.StaticKeyword))
		{
			return;
		}

		if (
			parameters[0] is not
			{
				Modifiers: var parameterModifiers,
				Identifier.ValueText: var parameterName
			} parameter
		)
		{
			return;
		}

		if (parameterModifiers.Count == 0
			|| parameterModifiers.Count != 0
			&& parameterModifiers.All(static parameter => parameter.RawKind != (int)SyntaxKind.ThisKeyword))
		{
			return;
		}

		if (parameterName is "this" or "@this")
		{
			return;
		}

		context.ReportDiagnostic(
			Diagnostic.Create(
				descriptor: SD0502,
				location: identifier.GetLocation(),
				messageArgs: null,
				additionalLocations: new[] { parameter.GetLocation() }
			)
		);
	}
}
