using System.Runtime.CompilerServices;

namespace System.Collections.Generic;

/// <summary>
/// Represents enumeration methods for flags defined in an enumeration type, marked with <see cref="FlagsAttribute"/>.
/// </summary>
/// <seealso cref="Enum"/>
/// <seealso cref="FlagsAttribute"/>
public static class EnumFlagEnumeration
{
	/// <summary>
	/// Get all possible flags that the current enumeration field set.
	/// </summary>
	/// <typeparam name="T">The type of the enumeration.</typeparam>
	/// <param name="this">The current enumeration type instance.</param>
	/// <returns>All flags.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the type isn't applied the attribute <see cref="FlagsAttribute"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FlagsEnumTypeFieldEnumerator<T> GetEnumerator<T>(this T @this) where T : unmanaged, Enum => new(@this);
}
