namespace Sudoku.Strategying.Validation;

/// <summary>
/// Represents a field that describes the severity of the failure.
/// </summary>
public enum Severity
{
	/// <summary>
	/// The placeholder of the severity.
	/// </summary>
	None,

	/// <summary>
	/// Indicates the severity is to report information.
	/// </summary>
	Info,

	/// <summary>
	/// Indicates the severity is to report warning.
	/// </summary>
	Warning,

	/// <summary>
	/// Indicates the severity is to report error.
	/// </summary>
	Error
}
