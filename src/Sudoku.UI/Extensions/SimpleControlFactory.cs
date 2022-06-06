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
	/// <param name="title">The title.</param>
	/// <param name="message">The message.</param>
	/// <returns>The result instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ContentDialog CreateErrorDialog(UIElement uiElement, string title, string message)
		=> new()
		{
			XamlRoot = uiElement.XamlRoot,
			Title = title,
			Content = message,
			CloseButtonText = R["Close"],
			DefaultButton = ContentDialogButton.Close
		};
}
