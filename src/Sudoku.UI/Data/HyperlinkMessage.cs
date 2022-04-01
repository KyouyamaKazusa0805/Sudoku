using System;

namespace Sudoku.UI.Data;

/// <inheritdoc/>
public sealed class HyperlinkMessage : InfoBarMessage
{
	/// <summary>
	/// Indicates the description text of the hyperlink. The default value is <see cref="string.Empty"/>.
	/// </summary>
	public string HyperlinkDescription { get; set; } = "";

	/// <summary>
	/// Indicates the hyperlink that the current <see cref="InfoBar"/> relates to.
	/// </summary>
	public Uri Hyperlink { get; set; } = null!;
}
