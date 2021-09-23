namespace Sudoku.UI.Data;

/// <summary>
/// Indicates a converter that converts the current instance to a value, which is available to display.
/// </summary>
/// <typeparam name="TControl">The type of the control.</typeparam>
public abstract class PreferenceItemConverter<TControl> where TControl : FrameworkElement
{
	/// <summary>
	/// Binds the setting and a control value to display.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="control">The control instance to apply the value.</param>
	/// <returns>A <see cref="bool"/> value indicating whether the bind operation is successful.</returns>
	public abstract bool Bind(object? value, TControl control);
}
