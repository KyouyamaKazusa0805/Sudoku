using Sudoku.XmlDocs.Data;

namespace Sudoku.XmlDocs.MemberInfo
{
	/// <summary>
	/// Indicates the information to introduce a type.
	/// </summary>
	/// <param name="CustomModifiers">The custom modifiers.</param>
	/// <param name="CustomAccessibility">The accessibility.</param>
	/// <param name="TypeName">The type name.</param>
	/// <param name="Summary">Indicates the "summary" block in the documentation comments.</param>
	/// <param name="Remarks">Indicates the "remarks" block in the documentation comments.</param>
	/// <param name="TypeParameters">Indicates the "typeparam" block in the documentation comments.</param>
	/// <param name="TypeParameterConstraints">Indicates the type parameter constraints.</param>
	/// <param name="Example">Indicates the "example" block in the documentation comments.</param>
	/// <param name="Exceptions">Indicates the "exception" block in the documentation comments.</param>
	/// <param name="SeeAlsos">Indicates the "seealso" block in the documentation comments.</param>
	public sealed record TypeInfo(
		CustomModifier CustomModifiers, CustomAccessibility CustomAccessibility,
		string TypeName, string Summary, string? Remarks, TypeParameterDetails[]? TypeParameters,
		TypeParameterConstraintDetails[]? TypeParameterConstraints, string? Example,
		ExceptionDetails[]? Exceptions, string[]? SeeAlsos
	);
}
