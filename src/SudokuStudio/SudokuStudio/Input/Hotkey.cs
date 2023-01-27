namespace SudokuStudio.Input;

/// <summary>
/// Defines a hotkey.
/// </summary>
/// <param name="Modifiers">
/// Indicates the modifier keys. If you want to enable multiple modifiers of this hotkey, just use <see langword="operator"/> |
/// to combine flags.
/// </param>
/// <param name="Key">Indicates the key.</param>
public readonly record struct Hotkey(VirtualKeyModifiers Modifiers, VirtualKey Key);
