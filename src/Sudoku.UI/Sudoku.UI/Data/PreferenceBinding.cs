namespace Sudoku.UI.Data;

/// <summary>
/// Provides a binding relation that binds a method that sets a value in the type <see cref="Preference"/>,
/// and a <see cref="Preference"/> instance.
/// </summary>
/// <param name="Control">The control.</param>
/// <param name="ItemSetter">
/// Indicates a serial of methods that sets an item in <see cref="Preference"/>.
/// </param>
/// <param name="ControlRestoration">
/// Indicates a serial of methods that restores the state of a control bound with the setting item.
/// </param>
/// <seealso cref="Preference"/>
internal readonly record struct PreferenceBinding(
	FrameworkElement Control,
	Action ItemSetter,
	Action<FrameworkElement> ControlRestoration
);