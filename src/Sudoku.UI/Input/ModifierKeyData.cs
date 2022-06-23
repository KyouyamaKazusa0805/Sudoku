namespace Sudoku.UI.Input;

/// <summary>
/// Provides a way to get information for pressing on modifier keys.
/// </summary>
/// <param name="ControlIsDown">Indicates whether the <see cref="VirtualKey.Control"/> key is pressed down.</param>
/// <param name="ShiftIsDown">Indicates whether the <see cref="VirtualKey.Shift"/> key is pressed down.</param>
/// <param name="AltIsDown">Indicates whether the <see cref="VirtualKey.Menu"/> key is pressed down.</param>
internal readonly record struct ModifierKeyData(bool ControlIsDown, bool ShiftIsDown, bool AltIsDown)
{
	/// <summary>
	/// Creates a <see cref="ModifierKeyData"/> instance from the current state.
	/// </summary>
	/// <returns>The <see cref="ModifierKeyData"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ModifierKeyData FromCurrentState()
		=> new(
			VirtualKey.Control.ModifierKeyIsDown(),
			VirtualKey.Shift.ModifierKeyIsDown(),
			VirtualKey.Menu.ModifierKeyIsDown()
		);
}
