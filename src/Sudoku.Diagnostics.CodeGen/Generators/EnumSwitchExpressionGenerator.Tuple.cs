namespace Sudoku.Diagnostics.CodeGen.Generators;

partial class EnumSwitchExpressionGenerator
{
	/// <summary>
	/// Defines a tuple that stores the basic information for a generation operation unit.
	/// </summary>
	/// <param name="TypeSymbol">Indicates a type symbol.</param>
	/// <param name="Key">Indicates a key of the generation.</param>
	/// <param name="RootAttributeData">The attribute data for the type.</param>
	/// <param name="ListOfFieldAttributesData">Indicates a list of field attributes data.</param>
	private readonly record struct Tuple(
		INamedTypeSymbol TypeSymbol, string Key, AttributeData RootAttributeData,
		IEnumerable<(IFieldSymbol FieldSymbol, AttributeData AttributeData)> ListOfFieldAttributesData);
}
