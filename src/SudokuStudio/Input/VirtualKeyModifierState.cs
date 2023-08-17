namespace SudokuStudio.Input;

/// <summary>
/// Defines a quadruple <see cref="bool"/>s indicating the key-down state of virtual key modifiers
/// defined by type <see cref="VirtualKeyModifiers"/>.
/// </summary>
/// <param name="IsControlKeyDown">Indicates whether the control key is input.</param>
/// <param name="IsShiftKeyDown">Indicates whether the shift key is input.</param>
/// <param name="IsAltKeyDown">Indicates whether the alt key is input.</param>
/// <param name="IsWindowsKeyDown">Indicates whether the windows key is input.</param>
/// <seealso cref="VirtualKeyModifiers"/>
public readonly record struct VirtualKeyModifierState(bool IsControlKeyDown, bool IsShiftKeyDown, bool IsAltKeyDown, bool IsWindowsKeyDown) :
	IEqualityOperators<VirtualKeyModifierState, VirtualKeyModifierState, bool>
{
	/// <summary>
	/// Indicates whether all modifier keys are not pressed.
	/// </summary>
	public bool AllFalse => this is (false, false, false, false);

	/// <summary>
	/// Indicates whether all modifier keys are pressed.
	/// </summary>
	public bool AllTrue => this is (true, true, true, true);


	/// <summary>
	/// Creates a <see cref="VirtualKeyModifiers"/> instance using the current data.
	/// </summary>
	/// <returns>A <see cref="VirtualKeyModifiers"/> instance.</returns>
	public VirtualKeyModifiers AsKeyModifiers()
	{
		var result = (VirtualKeyModifiers)0;
		result |= IsControlKeyDown ? VirtualKeyModifiers.Control : default;
		result |= IsShiftKeyDown ? VirtualKeyModifiers.Shift : default;
		result |= IsAltKeyDown ? VirtualKeyModifiers.Menu : default;
		result |= IsWindowsKeyDown ? VirtualKeyModifiers.Windows : default;

		return result;
	}

	/// <summary>
	/// Determines whether the current instance contains the specified virtual key modifier.
	/// </summary>
	/// <param name="modifier">The key modifier.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public bool HasModifier(VirtualKeyModifiers modifier) => (AsKeyModifiers() & modifier) == modifier;


	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	public static bool operator ==(VirtualKeyModifierState left, VirtualKeyModifiers right) => left.AsKeyModifiers() == right;

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Equality(TSelf, TOther)"/>
	public static bool operator ==(VirtualKeyModifiers left, VirtualKeyModifierState right) => left == right.AsKeyModifiers();

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	public static bool operator !=(VirtualKeyModifierState left, VirtualKeyModifiers right) => !(left == right);

	/// <inheritdoc cref="IEqualityOperators{TSelf, TOther, TResult}.op_Inequality(TSelf, TOther)"/>
	public static bool operator !=(VirtualKeyModifiers left, VirtualKeyModifierState right) => !(left == right);
}
