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
		if ((d, e) is not (FontPicker target, { NewValue: FontSerializationData n }))
		{
			return;
		}

		(target.SelectedFontName, target.SelectedFontScale, target.SelectedColor) = n;
	}
}
