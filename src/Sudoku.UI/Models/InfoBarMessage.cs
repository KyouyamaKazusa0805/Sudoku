namespace Sudoku.UI.Models;

/// <summary>
/// Defines a <see cref="InfoBarMessage"/> instance that represents for a message
/// that is used for the displaying as the message in <see cref="InfoBar"/> controls.
/// </summary>
/// <seealso cref="InfoBar"/>
public abstract class InfoBarMessage
{
	/// <summary>
	/// Indicates the message to be displayed.
	/// </summary>
	public required string Message { get; set; }

	/// <summary>
	/// Indicates the severity of the information.
	/// </summary>
	public required InfoBarSeverity Severity { get; set; }
}
