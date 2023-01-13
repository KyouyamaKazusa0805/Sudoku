namespace SudokuStudio.Input;

/// <summary>
/// Defines a quadruple <see cref="bool"/>s indicating the key-down status of virtual key modifiers
/// defined by type <see cref="winsys::VirtualKeyModifiers"/>.
/// </summary>
/// <param name="IsControlKeyDown">Indicates whether the control key is input.</param>
/// <param name="IsShiftKeyDown">Indicates whether the shift key is input.</param>
/// <param name="IsAltKeyDown">Indicates whether the alt key is input.</param>
/// <param name="IsWindowsKeyDown">Indicates whether the windows key is input.</param>
/// <seealso cref="winsys::VirtualKeyModifiers"/>
public readonly record struct VirtualKeyModifierStatus(bool IsControlKeyDown, bool IsShiftKeyDown, bool IsAltKeyDown, bool IsWindowsKeyDown) :
	IEquatable<VirtualKeyModifierStatus>,
	IEqualityOperators<VirtualKeyModifierStatus, VirtualKeyModifierStatus, bool>
{
	/// <summary>
	/// Creates a <see cref="winsys::VirtualKeyModifiers"/> instance using the current data.
	/// </summary>
	/// <returns>A <see cref="winsys::VirtualKeyModifiers"/> instance.</returns>
	public winsys::VirtualKeyModifiers AsKeyModifiers()
	{
		var result = (winsys::VirtualKeyModifiers)0;
		result |= IsControlKeyDown ? winsys::VirtualKeyModifiers.Control : default;
		result |= IsShiftKeyDown ? winsys::VirtualKeyModifiers.Shift : default;
		result |= IsAltKeyDown ? winsys::VirtualKeyModifiers.Menu : default;
		result |= IsWindowsKeyDown ? winsys::VirtualKeyModifiers.Windows : default;

		return result;
	}

	/// <summary>
	/// Determines whether the current instance contains the specified virtual key modifier.
	/// </summary>
	/// <param name="modifier">The key modifier.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public bool HasModifier(winsys::VirtualKeyModifiers modifier) => (AsKeyModifiers() & modifier) == modifier;


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	public static bool operator ==(VirtualKeyModifierStatus left, winsys::VirtualKeyModifiers right) => left.AsKeyModifiers() == right;

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	public static bool operator ==(winsys::VirtualKeyModifiers left, VirtualKeyModifierStatus right) => left == right.AsKeyModifiers();

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	public static bool operator !=(VirtualKeyModifierStatus left, winsys::VirtualKeyModifiers right) => !(left == right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	public static bool operator !=(winsys::VirtualKeyModifiers left, VirtualKeyModifierStatus right) => !(left == right);
}
