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
	public static unsafe T[] GetAllFlags<T>(this T @this) where T : unmanaged, Enum
	{
		// Create a buffer to gather all possible flags.
		scoped var buffer = (stackalloc T[Enum.GetValues<T>().Length]);
		var i = 0;
		foreach (var flag in @this)
		{
			buffer[i++] = flag;
		}

		if (i == 0)
		{
			return [];
		}

		// Returns the instance and copy the values.
		var result = new T[i];
		Unsafe.CopyBlock(ref Ref.AsByteRef(ref result[0]), in Ref.AsReadOnlyByteRef(in buffer[0]), (uint)(sizeof(T) * i));

		// Returns the value.
		return [.. result.Distinct()];
	}

	/// <summary>
	/// Determines whether one or more bit fields are set in the current instance.
	/// </summary>
	/// <typeparam name="T">The type of the enumeration.</typeparam>
	/// <param name="this">The current enumeration type instance.</param>
	/// <param name="other">The other instance to check.</param>
	/// <returns>
	/// <see langword="true"/> if the bit field or bit fields that are set in <paramref name="other"/>
	/// are also set in the current instance; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentException">Throws when the used bytes aren't 1, 2 or 4.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool Flags<T>(this T @this, T other) where T : unmanaged, Enum
	{
		return @this.GetTypeCode() switch
		{
			TypeCode.SByte => (fastConvert<sbyte>(@this) & fastConvert<sbyte>(other)) == fastConvert<sbyte>(other),
			TypeCode.Byte => (fastConvert<byte>(@this) & fastConvert<byte>(other)) == fastConvert<byte>(other),
			TypeCode.Int16 => (fastConvert<short>(@this) & fastConvert<short>(other)) == fastConvert<short>(other),
			TypeCode.UInt16 => (fastConvert<ushort>(@this) & fastConvert<ushort>(other)) == fastConvert<ushort>(other),
			TypeCode.Int32 => (fastConvert<int>(@this) & fastConvert<int>(other)) == fastConvert<int>(other),
			TypeCode.UInt32 => (fastConvert<uint>(@this) & fastConvert<uint>(other)) == fastConvert<uint>(other),
			TypeCode.Int64 => (fastConvert<long>(@this) & fastConvert<long>(other)) == fastConvert<long>(other),
			TypeCode.UInt64 => (fastConvert<ulong>(@this) & fastConvert<ulong>(other)) == fastConvert<ulong>(other),
			_ => throw new NotSupportedException("The specified underlying type is not supported.")
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static TResult fastConvert<TResult>(T value) => Unsafe.As<T, TResult>(ref value);
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
	public static FlagsEnumTypeFieldIterator<T> GetEnumerator<T>(this T @this) where T : unmanaged, Enum => new(@this);
}
