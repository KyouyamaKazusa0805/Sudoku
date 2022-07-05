namespace Sudoku.UI.Models;

/// <summary>
/// Indicates a keyboard pair.
/// </summary>
/// <param name="ModifierKeys">
/// The modifier keys. Use <c><see langword="operator"/> |</c> to combine multiple modifier keys.
/// </param>
/// <param name="VirtualKeys">The virtual keys.</param>
internal readonly record struct KeyboardPair(VirtualKeyModifiers ModifierKeys, VirtualKey[] VirtualKeys) :
	IEquatable<KeyboardPair>,
	IEqualityOperators<KeyboardPair, KeyboardPair>
{
	/// <summary>
	/// Initializes a <see cref="KeyboardPair"/> instance via the specified modifier keys and a virtual key.
	/// </summary>
	/// <param name="modifierKeys">The modifier keys.</param>
	/// <param name="virtualKey">The virtual key.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public KeyboardPair(VirtualKeyModifiers modifierKeys, VirtualKey virtualKey) :
		this(modifierKeys, new[] { virtualKey })
	{
	}
}
