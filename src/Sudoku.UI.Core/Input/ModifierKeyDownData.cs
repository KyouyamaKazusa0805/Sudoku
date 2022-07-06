namespace Sudoku.UI.Input;

/// <summary>
/// Provides a way to get information for pressing on modifier keys.
/// </summary>
/// <param name="ControlIsDown">Indicates whether the <see cref="Key.Control"/> key is pressed down.</param>
/// <param name="ShiftIsDown">Indicates whether the <see cref="Key.Shift"/> key is pressed down.</param>
/// <param name="AltIsDown">Indicates whether the <see cref="Key.Menu"/> key is pressed down.</param>
public readonly record struct ModifierKeyDownData(bool ControlIsDown, bool ShiftIsDown, bool AltIsDown) :
	IEquatable<ModifierKeyDownData>,
	IEqualityOperators<ModifierKeyDownData, ModifierKeyDownData>
{
	/// <summary>
	/// Indicates the sync root.
	/// </summary>
	private static readonly object SyncRoot = new();


	/// <summary>
	/// Creates a <see cref="ModifierKeyDownData"/> instance from the current state.
	/// </summary>
	/// <returns>The <see cref="ModifierKeyDownData"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ModifierKeyDownData FromCurrentState()
	{
		lock (SyncRoot)
		{
			return new(Key.Control.ModifierKeyIsDown(), Key.Shift.ModifierKeyIsDown(), Key.Menu.ModifierKeyIsDown());
		}
	}
}
