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
public readonly record struct VirtualKeyModifierStatus(bool IsControlKeyDown, bool IsShiftKeyDown, bool IsAltKeyDown, bool IsWindowsKeyDown);
