namespace Sudoku.UI.Models;

/// <summary>
/// Indicates a keyboard pair.
/// </summary>
/// <param name="ModifierKeys">
/// The modifier keys. Use <c><see langword="operator"/> |</c> to combine multiple modifier keys.
/// </param>
/// <param name="VirtualKey">The virtual key.</param>
internal readonly record struct KeyboardPair(VirtualKeyModifiers ModifierKeys, VirtualKey VirtualKey);