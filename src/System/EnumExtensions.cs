namespace System;

/// <summary>
/// Provides extension methods on <see cref="Enum"/>.
/// </summary>
/// <seealso cref="Enum"/>
public static class EnumExtensions
{
	/// <summary>
	/// Checks whether the current enumeration field is a flag.
	/// </summary>
	/// <typeparam name="T">The type of the current field.</typeparam>
	/// <param name="this">The current field to check.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe bool IsFlag<T>(this T @this) where T : unmanaged, Enum
		=> sizeof(T) switch
		{
			1 or 2 or 4 when Unsafe.As<T, int>(ref @this) is var l => (l & l - 1) == 0,
			8 when Unsafe.As<T, long>(ref @this) is var l => (l & l - 1) == 0,
			_ => false
		};

	/// <summary>
	/// To get all possible flags from a specified enumeration instance.
	/// </summary>
	/// <typeparam name="T">The type of that enumeration.</typeparam>
	/// <param name="this">The field.</param>
	/// <returns>
	/// All flags. If the enumeration field doesn't contain any flags, the return value will be <see langword="null"/>.
	/// </returns>
	public static ReadOnlySpan<T> GetAllFlags<T>(this T @this) where T : unmanaged, Enum
	{
		var set = new HashSet<T>(Enum.GetValues<T>().Length);
		foreach (var flag in @this)
		{
			set.Add(flag);
		}
		return (T[])[.. set];
	}

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
	public static EnumFlagsEnumerator<T> GetEnumerator<T>(this T @this) where T : unmanaged, Enum => new(@this);
}
