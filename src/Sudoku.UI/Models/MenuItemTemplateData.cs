namespace Sudoku.UI.Models;

/// <summary>
/// Indicates the template data used for displaying a menu item.
/// </summary>
public sealed class MenuItemTemplateData
{
	/// <summary>
	/// Indicates the tag used.
	/// </summary>
	public object Tag { get; set; } = null!;

	/// <summary>
	/// Indicates whether the current menu item is enabled, which means it can interact with users.
	/// The default value is <see langword="true"/>.
	/// </summary>
	public bool IsEnabled { get; set; } = true;

	/// <summary>
	/// The title.
	/// </summary>
	public object Title { get; set; } = null!;

	/// <summary>
	/// The icon used. The default value is <c>(<see cref="Microsoft.UI.Xaml.Controls.Symbol"/>)0</c>.
	/// </summary>
	public Symbol Symbol { get; set; }

	/// <summary>
	/// Indicates the default visibility. The default value is <see cref="Visibility.Visible"/>.
	/// </summary>
	public Visibility Visibility { get; set; } = Visibility.Visible;
}
