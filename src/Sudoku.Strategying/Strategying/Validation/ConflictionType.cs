namespace Sudoku.Strategying.Validation;

/// <summary>
/// Indicates the confliction type.
/// </summary>
public enum ConflictionType
{
	/// <summary>
	/// Indicates no confliction raised.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the value is fully covered by the other <see cref="Constraint"/>.
	/// </summary>
	ValueFullyCovered,

	/// <summary>
	/// Indicates the value differs.
	/// </summary>
	ValueDiffers
}
