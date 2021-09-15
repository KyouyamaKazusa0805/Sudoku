namespace Sudoku.CodeGenerating.Generators;

/// <summary>
/// Indicates the source generator that generates the code about the attributes to mark onto
/// the method <c>Formatize</c> in the type <c>StepInfo</c>.
/// </summary>
#if false
[Generator]
#endif
public sealed class StepInfoFormatizeGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (context.IsNotInProject(ProjectNames.Solving))
		{
			return;
		}

		Func<ISymbol?, ISymbol?, bool> f = SymbolEqualityComparer.Default.Equals;
		var compilation = context.Compilation;
		var symbol = compilation.GetTypeByMetadataName("Sudoku.Solving.Manual.StepInfo");
		var attributeSymbol = compilation.GetTypeByMetadataName("Sudoku.Solving.Text.FormatItemAttribute");

		string attributes = string.Join(
			"\r\n\t",
			from INamedTypeSymbol type in compilation.GetSymbolsWithName(static _ => true, SymbolFilter.Type, context.CancellationToken)
			where type is { IsAbstract: false, IsGenericType: false }
			let baseType = type.GetBaseTypes()
			where baseType.Any(baseType => f(baseType, symbol))
			let properties = type.GetMembers().OfType<IPropertySymbol>()
			let fullName = type.ToDisplayString(FormatOptions.TypeFormat)
			where properties.Any(p => p.GetAttributes().Any(a => f(a.AttributeClass, attributeSymbol)))
			select $"[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicProperties, typeof({fullName}), Condition = \"SOLUTION_WIDE_CODE_ANALYSIS\")]"
		);

		context.AddSource(
			"Sudoku.Solving.Manual.StepInfo",
			"DynamicDependencies",
			$@"#pragma warning disable 1591

using global::System.Diagnostics.CodeAnalysis;

namespace Sudoku.Solving.Manual;

partial record class StepInfo
{{
	{attributes}
	public partial string Formatize(bool handleEscaping = false);
}}
"
		);
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
