namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a field that describes why the validation failed.
/// </summary>
public enum ValidationFailedReason
{
	/// <summary>
	/// The placeholder of this enumeration type.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the enumeration field is not defined.
	/// </summary>
	EnumerationFieldNotDefined,

	/// <summary>
	/// Indicates the value is out of range.
	/// </summary>
	OutOfRange,

	/// <summary>
	/// Indicates the value will make the condition always true.
	/// </summary>
	AlwaysTrue,

	/// <summary>
	/// Indicates the value will make the condition always false.
	/// </summary>
	AlwaysFalse,

	/// <summary>
	/// Indicates the conditition is too strict, causing little puzzles can be checked.
	/// </summary>
	TooStrict
}
