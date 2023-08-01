namespace SudokuStudio.Input;

/// <summary>
/// Defines a hotkey.
/// </summary>
/// <param name="Modifiers">
/// Indicates the modifier keys. If you want to enable multiple modifiers of this hotkey,
/// just use <see cref="VirtualKeyModifiers"/>.<see langword="operator"/> <c>|</c> to combine flags.
/// </param>
/// <param name="Key">Indicates the key.</param>
public readonly record struct Hotkey(VirtualKeyModifiers Modifiers, VirtualKey Key)
{
	/// <summary>
	/// Initializes a <see cref="Hotkey"/> instance via the specified key.
	/// </summary>
	/// <param name="key">The virtual key.</param>
	public Hotkey(VirtualKey key) : this(VirtualKeyModifiers.None, key)
	{
	}
}
