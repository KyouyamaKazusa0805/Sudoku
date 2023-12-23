namespace SudokuStudio.Input;

/// <summary>
/// Defines a hotkey.
/// </summary>
/// <param name="Key">Indicates the key.</param>
/// <param name="Modifiers">
/// Indicates the modifier keys. If you want to enable multiple modifiers of this hotkey,
/// just use <see cref="VirtualKeyModifiers"/>.<see langword="operator"/> <c>|</c> to combine flags.
/// By default the value is <see cref="VirtualKeyModifiers.None"/>.
/// </param>
public readonly record struct Hotkey(VirtualKey Key, VirtualKeyModifiers Modifiers = VirtualKeyModifiers.None);
