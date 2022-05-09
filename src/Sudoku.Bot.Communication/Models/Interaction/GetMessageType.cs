namespace Sudoku.Bot.Communication.Models;

/// <summary>
/// Indicates the type for defining the message getting. The field names in this type cannot be modified.
/// </summary>
public enum GetMessageType
{
	/// <summary>
	/// Indicates getting messages before and after the specified ID value.
	/// </summary>
	Around,

	/// <summary>
	/// Indicates getting messages before the specified ID value.
	/// </summary>
	Before,

	/// <summary>
	/// Indicates getting messages after the specified ID value.
	/// </summary>
	After,

	/// <summary>
	/// Indicates getting messages at latest.
	/// </summary>
	Latest
}
