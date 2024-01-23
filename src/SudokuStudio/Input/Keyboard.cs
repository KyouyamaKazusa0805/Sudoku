namespace SudokuStudio.Input;

/// <summary>
/// Provides with a easier way to visit key-down state.
/// </summary>
public static class Keyboard
{
	/// <summary>
	/// Checks whether the key <see cref="VirtualKey.Control"/> is input.
	/// </summary>
	public static bool IsControlKeyDown => InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Control).HasFlag(CoreVirtualKeyStates.Down);

	/// <summary>
	/// Checks whether the key <see cref="VirtualKey.Shift"/> is input.
	/// </summary>
	public static bool IsShiftKeyDown => InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Shift).HasFlag(CoreVirtualKeyStates.Down);

	/// <summary>
	/// Checks whether the key <see cref="VirtualKey.Menu"/> is input.
	/// </summary>
	public static bool IsAltKeyDown => InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.Menu).HasFlag(CoreVirtualKeyStates.Down);

	/// <summary>
	/// Checks whether the key <see cref="VirtualKey.LeftWindows"/> or <see cref="VirtualKey.RightWindows"/> is input.
	/// </summary>
	public static bool IsWindowsKeyDown
		=> InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.LeftWindows).HasFlag(CoreVirtualKeyStates.Down)
		|| InputKeyboardSource.GetKeyStateForCurrentThread(VirtualKey.RightWindows).HasFlag(CoreVirtualKeyStates.Down);


	/// <summary>
	/// Determines which digit is input.
	/// </summary>
	/// <param name="virtualKey">The number input key.</param>
	/// <returns>
	/// A <see cref="Digit"/> value corresponding to the key. Cases:
	/// <list type="table">
	/// <listheader>
	/// <term>Return value</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term><![CDATA[>= 0 and <= 9]]></term>
	/// <description>Valid digit inputs</description>
	/// </item>
	/// <item>
	/// <term>-1</term>
	/// <description>
	/// Inputting <see cref="VirtualKey.Number0"/>, <see cref="VirtualKey.NumberPad0"/>,
	/// <see cref="VirtualKey.Back"/> or <see cref="VirtualKey.Delete"/>
	/// </description>
	/// </item>
	/// <item>
	/// <term>-2</term>
	/// <description>Other invalid inputs</description>
	/// </item>
	/// </list>
	/// </returns>
	public static Digit GetInputDigit(VirtualKey virtualKey)
		=> virtualKey switch
		{
			VirtualKey.Number0 or VirtualKey.NumberPad0 => -1,
			VirtualKey.Back or VirtualKey.Delete => -1,
			>= VirtualKey.Number1 and <= VirtualKey.Number9 => virtualKey - VirtualKey.Number1,
			>= VirtualKey.NumberPad1 and <= VirtualKey.NumberPad9 => virtualKey - VirtualKey.NumberPad1,
			_ => -2
		};

	/// <summary>
	/// Try to create a <see cref="VirtualKeyModifierState"/> instance to determine key-down state.
	/// </summary>
	/// <returns>A <see cref="VirtualKeyModifierState"/> instance.</returns>
	/// <remarks>
	/// Please note that type <see cref="VirtualKeyModifierState"/> is a <see langword="readonly record struct"/>,
	/// which means you can use deconstruction syntax to check what key you want to check:
	/// <code><![CDATA[
	/// var (_, shiftIsDown, _, _) = Keyboard.GetModifierStateForCurrentThread();
	/// ]]></code>
	/// For more information, please visit that type to learn more details.
	/// </remarks>
	public static VirtualKeyModifierState GetModifierStateForCurrentThread() => new(IsControlKeyDown, IsShiftKeyDown, IsAltKeyDown, IsWindowsKeyDown);
}
