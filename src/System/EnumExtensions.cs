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
	public static bool IsFlag<T>(this T @this) where T : unmanaged, Enum
	{
		return new Dictionary<Type, Func<bool>>
		{
			{ typeof(sbyte), f<sbyte> },
			{ typeof(byte), f<byte> },
			{ typeof(short), f<short> },
			{ typeof(ushort), f<ushort> },
			{ typeof(int), f<int> },
			{ typeof(uint), f<uint> },
			{ typeof(long), f<long> },
			{ typeof(ulong), f<ulong> },
		}.TryGetValue(Enum.GetUnderlyingType(typeof(T)), out var func) && func();


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		bool f<TInteger>() where TInteger : IBinaryInteger<TInteger>
			=> Unsafe.As<T, TInteger>(ref @this) is var integer && (integer == TInteger.Zero || TInteger.IsPow2(integer));
	}

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
