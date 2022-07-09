namespace Sudoku.UI;

/// <summary>
/// Provides a simple way to construct controls.
/// </summary>
internal static class SimpleControlFactory
{
	/// <summary>
	/// Creates a <see cref="ContentDialog"/> instance.
	/// </summary>
	/// <param name="uiElement">The UI control.</param>
	/// <returns>The result instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ContentDialog CreateErrorDialog(UIElement uiElement)
		=> new()
		{
			XamlRoot = uiElement.XamlRoot,
			CloseButtonText = R["Close"]!,
			DefaultButton = ContentDialogButton.Close
		};

	/// <summary>
	/// Creates a <see cref="ContentDialog"/> instance.
	/// </summary>
	/// <param name="uiElement">The UI control.</param>
	/// <param name="title">The title.</param>
	/// <param name="message">The message.</param>
	/// <returns>The result instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ContentDialog CreateErrorDialog(UIElement uiElement, string title, string message)
		=> new ContentDialog()
		{
			XamlRoot = uiElement.XamlRoot,
			CloseButtonText = R["Close"]!,
			DefaultButton = ContentDialogButton.Close
		}
		.WithTitle(title)
		.WithContent(message);
}
