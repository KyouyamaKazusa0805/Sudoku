namespace SudokuStudio.Input;

/// <summary>
/// Provides with a easier way to visit key-down status.
/// </summary>
public static class Keyboard
{
	/// <summary>
	/// Checks whether the key <see cref="winsys::VirtualKey.Control"/> is input.
	/// </summary>
	public static bool IsControlKeyDown
		=> InputKeyboardSource.GetKeyStateForCurrentThread(winsys::VirtualKey.Control).Flags(CoreVirtualKeyStates.Down);

	/// <summary>
	/// Checks whether the key <see cref="winsys::VirtualKey.Shift"/> is input.
	/// </summary>
	public static bool IsShiftKeyDown
		=> InputKeyboardSource.GetKeyStateForCurrentThread(winsys::VirtualKey.Shift).Flags(CoreVirtualKeyStates.Down);

	/// <summary>
	/// Checks whether the key <see cref="winsys::VirtualKey.Menu"/> is input.
	/// </summary>
	public static bool IsAltKeyDown
		=> InputKeyboardSource.GetKeyStateForCurrentThread(winsys::VirtualKey.Menu).Flags(CoreVirtualKeyStates.Down);

	/// <summary>
	/// Checks whether the key <see cref="winsys::VirtualKey.LeftWindows"/> or <see cref="winsys::VirtualKey.RightWindows"/> is input.
	/// </summary>
	public static bool IsWindowsKeyDown
		=> InputKeyboardSource.GetKeyStateForCurrentThread(winsys::VirtualKey.LeftWindows).Flags(CoreVirtualKeyStates.Down)
		|| InputKeyboardSource.GetKeyStateForCurrentThread(winsys::VirtualKey.RightWindows).Flags(CoreVirtualKeyStates.Down);


	/// <summary>
	/// Determines which digit is input.
	/// </summary>
	/// <param name="virtualKey">The number input key.</param>
	/// <returns>
	/// An <see cref="int"/> value corresponding to the key. -1 for inputting <see cref="winsys::VirtualKey.Number0"/>
	/// or <see cref="winsys::VirtualKey.NumberPad0"/> and -2 for invalid inputs.
	/// </returns>
	public static int GetInputDigit(winsys::VirtualKey virtualKey)
		=> virtualKey switch
		{
			>= winsys::VirtualKey.Number1 and <= winsys::VirtualKey.Number9 => virtualKey - winsys::VirtualKey.Number1,
			>= winsys::VirtualKey.NumberPad1 and <= winsys::VirtualKey.NumberPad9 => virtualKey - winsys::VirtualKey.NumberPad1,
			winsys::VirtualKey.Number0 or winsys::VirtualKey.NumberPad0 => -1,
			_ => -2
		};

	/// <summary>
	/// Try to create a <see cref="VirtualKeyModifierStatus"/> instance to determine key-down status.
	/// </summary>
	/// <returns>A <see cref="VirtualKeyModifierStatus"/> instance.</returns>
	/// <remarks>
	/// Please note that type <see cref="VirtualKeyModifierStatus"/> is a <see langword="readonly record struct"/>,
	/// which means you can use deconstruction syntax to check what key you want to check:
	/// <code><![CDATA[
	/// var (_, shiftIsDown, _, _) = Keyboard.GetModifierStatusForCurrentThread();
	/// ]]></code>
	/// For more information, please visit that type to learn more details.
	/// </remarks>
	public static VirtualKeyModifierStatus GetModifierStatusForCurrentThread()
		=> new(IsControlKeyDown, IsShiftKeyDown, IsAltKeyDown, IsWindowsKeyDown);
}
