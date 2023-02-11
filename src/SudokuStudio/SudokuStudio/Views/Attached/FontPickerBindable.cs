namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="FontPicker"/> instances.
/// </summary>
/// <seealso cref="FontPicker"/>
[AttachedProperty<FontSerializationData>(nameof(FontPicker.SelectedFontData), CallbackMethodName = nameof(SelectedFontDataPropertyCallback))]
public static partial class FontPickerBindable
{
	private static void SelectedFontDataPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if ((d, e) is (FontPicker target, { NewValue: FontSerializationData n }))
		{
			(target.SelectedFontName, target.SelectedFontScale, target.SelectedColor) = n;
		}
	}
}
