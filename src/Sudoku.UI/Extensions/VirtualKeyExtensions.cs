namespace Windows.System;

/// <summary>
/// Provides extension methods on <see cref="VirtualKey"/>.
/// </summary>
/// <seealso cref="VirtualKey"/>
public static class VirtualKeyExtensions
{
	/// <summary>
	/// Determines whether the specified modifier key is pressed down.
	/// </summary>
	/// <param name="this">The modifier key.</param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ModifierKeyIsDown(this VirtualKey @this)
		=> InputKeyboardSource.GetKeyStateForCurrentThread(@this).Flags(CoreVirtualKeyStates.Down);
}
