namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="FontPicker"/> instances.
/// </summary>
/// <seealso cref="FontPicker"/>
[AttachedProperty<FontSerializationData>(nameof(FontPicker.SelectedFontData))]
public static partial class FontPickerBindable
{
	[Callback]
	private static void SelectedFontDataPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if (
#pragma warning disable format
			(d, e) is not (
				FontPicker target,
				{
					NewValue: FontSerializationData
					{
						FontName: var fontName,
						FontScale: var fontScale,
						FontColor: var fontColor
					}
				}
			)
#pragma warning restore format
		)
		{
			return;
		}

		target.SelectedFontName = fontName;
		target.SelectedFontScale = fontScale;
		target.SelectedColor = fontColor;
	}
}
