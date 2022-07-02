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

	/// <summary>
	/// Creates a <see cref="FileSavePicker"/> that outputs for a picture.
	/// </summary>
	/// <returns>The <see cref="FileSavePicker"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileSavePicker PictureFileSavePicker()
		=> new FileSavePicker()
			.WithDefaultFileExtension(CommonFileExtensions.PortablePicture)
			.WithSuggestedFileName(R["Sudoku"]!)
			.AddFileTypeChoice(R["FileExtension_Picture"]!, CommonFileExtensions.PortablePicture);
}
