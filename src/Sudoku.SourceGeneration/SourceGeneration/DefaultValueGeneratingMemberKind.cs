namespace Sudoku.SourceGeneration;

/// <summary>
/// Defines a kind of default value generating member.
/// </summary>
internal enum DefaultValueGeneratingMemberKind
{
	/// <summary>
	/// Indicates the member type is a field.
	/// </summary>
	Field,

	/// <summary>
	/// Indicates the member type is a property.
	/// </summary>
	Property,

	/// <summary>
	/// Indicates the member type is a parameterless method.
	/// </summary>
	ParameterlessMethod,

	/// <summary>
	/// Indicates the member cannot be referenced.
	/// </summary>
	CannotReference,

	/// <summary>
	/// Otherwise.
	/// </summary>
	Otherwise
}
