namespace Sudoku.UI.Data;

/// <summary>
/// Defines a set of information that is used by creating an <see cref="InfoBar"/>.
/// Different with <see cref="InfoBarInfo"/>, this type contains two extra properties
/// <see cref="Hyperlink"/> and <see cref="HyperlinkDescription"/>, to describe the hyperlink related.
/// </summary>
/// <seealso cref="InfoBar"/>
/// <seealso cref="Hyperlink"/>
/// <seealso cref="HyperlinkDescription"/>
public sealed class InfoBarInfoWithLink
{
	/// <summary>
	/// Indicates the message to be displayed. The default value is <see cref="string.Empty"/>.
	/// </summary>
	public string Message { get; set; } = "";

	/// <summary>
	/// Indicates the description text of the hyperlink. The default value is <see cref="string.Empty"/>.
	/// </summary>
	public string HyperlinkDescription { get; set; } = "";

	/// <summary>
	/// Indicates the severity of the information.
	/// </summary>
	public InfoBarSeverity Severity { get; set; }

	/// <summary>
	/// Indicates the hyperlink that the current <see cref="InfoBar"/> relates to.
	/// </summary>
	public Uri Hyperlink { get; set; } = null!;
}
