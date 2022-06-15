namespace Sudoku.UI.Models;

/// <inheritdoc/>
public sealed class HyperlinkMessage : InfoBarMessage
{
	/// <summary>
	/// Indicates the description text of the hyperlink.
	/// </summary>
	public required string HyperlinkDescription { get; set; }

	/// <summary>
	/// Indicates the hyperlink that the current <see cref="InfoBar"/> relates to.
	/// </summary>
	public required Uri Hyperlink { get; set; }
}
