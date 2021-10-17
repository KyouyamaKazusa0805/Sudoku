namespace Sudoku.Diagnostics.CodeAnalysis.Providers;

[PrivatizeParameterlessConstructor]
internal sealed partial class GeneratorSyntaxContext_Dap
{
	[DeconstructArgumentProvider]
	internal static IOperation? Operation(GeneratorSyntaxContext generatorSyntaxContext) =>
		generatorSyntaxContext.SemanticModel.GetOperation(generatorSyntaxContext.Node);
}
