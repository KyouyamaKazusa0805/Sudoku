namespace SudokuStudio.Views.Attached;

/// <summary>
/// Defines a bind behaviors on <see cref="FontPicker"/> instances.
/// </summary>
/// <seealso cref="FontPicker"/>
public static class FontPickerBindable
{
	/// <summary>
	/// Defines a dependency property instance that is bindable selected font data.
	/// </summary>
	public static readonly DependencyProperty SelectedFontDataProperty =
		DependencyProperty.RegisterAttached(
			nameof(FontPicker.SelectedFontData),
			typeof(FontSerializationData),
			typeof(FontPickerBindable),
			new(
				null,
				static (d, e) =>
				{
					if ((d, e) is (FontPicker target, { NewValue: FontSerializationData n }))
					{
						(target.SelectedFontName, target.SelectedFontScale, target.SelectedColor) = n;
					}
				}
			)
		);


	/// <summary>
	/// Gets a <see cref="FontSerializationData"/> instance.
	/// </summary>
	/// <param name="obj">The dependency object.</param>
	/// <returns>A <see cref="FontSerializationData"/> instance.</returns>
	public static FontSerializationData GetSelectedFontData(DependencyObject obj)
		=> (FontSerializationData)obj.GetValue(SelectedFontDataProperty);

	/// <summary>
	/// Sets a <see cref="FontSerializationData"/> instance.
	/// This will trigger the event to append inlines into the target <see cref="FontPicker"/>.
	/// </summary>
	/// <param name="obj">The dependency object.</param>
	/// <param name="value">The inlines to be added.</param>
	public static void SetSelectedFontData(DependencyObject obj, FontSerializationData value)
		=> obj.SetValue(SelectedFontDataProperty, value);
}
