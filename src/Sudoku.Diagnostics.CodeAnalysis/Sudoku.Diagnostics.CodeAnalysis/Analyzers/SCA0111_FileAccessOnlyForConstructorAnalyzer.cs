namespace Sudoku.Diagnostics.CodeAnalysis.Analyzers;

[SupportedDiagnostics("SCA0001", "SCA0111")]
[RegisterOperationAction(nameof(AnalysisContext.RegisterSyntaxNodeAction), typeof(SyntaxKind), new[] { nameof(SyntaxKind.ObjectCreationExpression), nameof(SyntaxKind.ImplicitObjectCreationExpression) })]
public sealed partial class SCA0111_FileAccessOnlyForConstructorAnalyzer : DiagnosticAnalyzer
{
	private static partial void AnalyzeCore(SyntaxNodeAnalysisContext context)
	{
		if (context is not
			{
				CancellationToken: var ct,
				Compilation: var compilation,
				Node: BaseObjectCreationExpressionSyntax { SyntaxTree.FilePath: var referencingFilePath } node,
				SemanticModel: var semanticModel
			})
		{
			return;
		}

		if (semanticModel.GetOperation(node, ct) is not IObjectCreationOperation
			{
				Constructor:
				{
					DeclaringSyntaxReferences: [{ SyntaxTree.FilePath: var declaringFilePath }]
				} constructorSymbol
			})
		{
			return;
		}

		var location = node.GetLocation();
		var attribute = compilation.GetTypeByMetadataName(SpecialFullTypeNames.FileAccessOnlyAttribute);
		if (attribute is null)
		{
			context.ReportDiagnostic(Diagnostic.Create(SCA0001, location, messageArgs: new[] { SpecialFullTypeNames.FileAccessOnlyAttribute }));
			return;
		}

		if (constructorSymbol.GetAttributes().All(a => !SymbolEqualityComparer.Default.Equals(a.AttributeClass, attribute)))
		{
			return;
		}

		if (declaringFilePath == referencingFilePath)
		{
			return;
		}

		context.ReportDiagnostic(Diagnostic.Create(SCA0111, location));
	}
}
