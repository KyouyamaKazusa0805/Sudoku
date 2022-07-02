namespace Sudoku.UI.Input;

/// <summary>
/// Provides a way to get information for pressing on modifier keys.
/// </summary>
/// <param name="ControlIsDown">Indicates whether the <see cref="VirtualKey.Control"/> key is pressed down.</param>
/// <param name="ShiftIsDown">Indicates whether the <see cref="VirtualKey.Shift"/> key is pressed down.</param>
/// <param name="AltIsDown">Indicates whether the <see cref="VirtualKey.Menu"/> key is pressed down.</param>
public readonly record struct ModifierKeyDownData(bool ControlIsDown, bool ShiftIsDown, bool AltIsDown) :
	IEquatable<ModifierKeyDownData>,
	IEqualityOperators<ModifierKeyDownData, ModifierKeyDownData>
{
	/// <summary>
	/// Creates a <see cref="ModifierKeyDownData"/> instance from the current state.
	/// </summary>
	/// <returns>The <see cref="ModifierKeyDownData"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ModifierKeyDownData FromCurrentState()
		=> new(
			VirtualKey.Control.ModifierKeyIsDown(),
			VirtualKey.Shift.ModifierKeyIsDown(),
			VirtualKey.Menu.ModifierKeyIsDown()
		);
}
