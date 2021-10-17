namespace Sudoku.Diagnostics.CodeAnalysis.Providers;

internal sealed class GeneratorSyntaxContext_Dap
{
	[DeconstructArgumentProvider]
	internal static IOperation? Operation(GeneratorSyntaxContext generatorSyntaxContext) =>
		generatorSyntaxContext.SemanticModel.GetOperation(generatorSyntaxContext.Node);
}
