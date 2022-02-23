namespace Sudoku.UI.Data;

/// <summary>
/// Defines a set of information that is used by creating an <see cref="InfoBar"/>.
/// </summary>
/// <seealso cref="InfoBar"/>
public sealed class InfoBarInfo
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
