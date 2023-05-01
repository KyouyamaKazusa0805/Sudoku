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
		if ((d, e) is not (FontPicker target, { NewValue: FontSerializationData { FontName: var n, FontScale: var s, FontColor: var c } }))
		{
			return;
		}

		target.SelectedFontName = n;
		target.SelectedFontScale = s;
		target.SelectedColor = c;
	}
}
