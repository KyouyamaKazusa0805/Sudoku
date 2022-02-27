namespace Sudoku.UI.Data;

/// <summary>
/// Defines a <see cref="InfoBarMessage"/> instance that represents for a message
/// that is used for the displaying as the message in <see cref="InfoBar"/> controls.
/// </summary>
/// <seealso cref="InfoBar"/>
public abstract class InfoBarMessage
{
	/// <summary>
	/// Indicates the message to be displayed. The default value is <see cref="string.Empty"/>.
	/// </summary>
	public string Message { get; set; } = "";

	/// <summary>
	/// Indicates the severity of the information.
	/// </summary>
	public InfoBarSeverity Severity { get; set; }
}
