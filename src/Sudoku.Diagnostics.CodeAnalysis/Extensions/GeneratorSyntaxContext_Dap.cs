namespace Sudoku.Diagnostics.CodeAnalysis.Extensions;

internal sealed class GeneratorSyntaxContext_Dap
{
	[DeconstructArgumentProvider]
	internal static IOperation? Operation(GeneratorSyntaxContext generatorSyntaxContext) =>
		generatorSyntaxContext.SemanticModel.GetOperation(generatorSyntaxContext.Node);
}
