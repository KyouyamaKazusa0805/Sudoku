namespace Sudoku.CodeGenerating.Generators;

partial class PrimaryConstructorGenerator
{
	/// <summary>
	/// Indicates the inner member symbol information quadruple.
	/// </summary>
	/// <param name="Type">Indicates the type name.</param>
	/// <param name="ParameterName">Indicates the parameter name.</param>
	/// <param name="Name">Indicates the name.</param>
	/// <param name="Attributes">Indicates all attributes that the type has marked.</param>
	private readonly record struct SymbolInfo(
		string Type, string ParameterName, string Name, IEnumerable<AttributeData> Attributes
	);
}
