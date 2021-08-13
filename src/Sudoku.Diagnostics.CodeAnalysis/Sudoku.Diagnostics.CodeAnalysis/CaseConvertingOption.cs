namespace Sudoku.Diagnostics.CodeAnalysis;

/// <summary>
/// Defines an option that uses and specifies the case converting, which is used in
/// <see cref="IdentifierMarshal.ToCamelCase(string, CaseConvertingOption)"/>
/// or <see cref="IdentifierMarshal.ToPascalCase(string)"/>.
/// </summary>
/// <seealso cref="IdentifierMarshal.ToCamelCase(string, CaseConvertingOption)"/>
/// <seealso cref="IdentifierMarshal.ToPascalCase(string)"/>
public enum CaseConvertingOption
{
	/// <summary>
	/// Indicates the option is none.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the option will specify that the leading underscore in the identifier will be reserved
	/// after conversion from a case to another case, e.g. Pascal case to camel case.
	/// </summary>
	ReserveLeadingUnderscore
}
