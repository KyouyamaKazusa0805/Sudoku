namespace System;

/// <summary>
/// Provides extension methods on <see cref="Enum"/>.
/// </summary>
/// <seealso cref="Enum"/>
public static unsafe class EnumExtensions
{
	/// <summary>
	/// Checks whether the current enumeration field is a flag.
	/// </summary>
	/// <typeparam name="TEnum">The type of the current field.</typeparam>
	/// <param name="this">The current field to check.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsFlag<TEnum>(this TEnum @this) where TEnum : unmanaged, Enum =>
		sizeof(TEnum) switch
		{
			1 or 2 or 4 when Unsafe.As<TEnum, int>(ref @this) is var l => (l & l - 1) == 0,
			8 when Unsafe.As<TEnum, long>(ref @this) is var l => (l & l - 1) == 0,
			_ => false
		};

	/// <summary>
	/// To get all possible flags from a specified enumeration instance.
	/// </summary>
	/// <typeparam name="TEnum">The type of that enumeration.</typeparam>
	/// <param name="this">The field.</param>
	/// <returns>
	/// All flags. If the enumeration field doesn't contain any flags,
	/// the return value will be <see langword="null"/>.
	/// </returns>
	public static TEnum[]? GetAllFlags<TEnum>(this TEnum @this) where TEnum : unmanaged, Enum
	{
		// Create a buffer to record all possible flags.
		var buffer = stackalloc TEnum[Enum.GetValues<TEnum>().Length];
		int i = 0;
		foreach (var flag in @this)
		{
			buffer[i++] = flag;
		}

		if (i == 0)
		{
			return null;
		}

		// Returns the instance and copy the values.
		var result = new TEnum[i];
		fixed (TEnum* ptr = result)
		{
			Unsafe.CopyBlock(ptr, buffer, (uint)(sizeof(TEnum) * i));
		}

		// Returns the value.
		return result;
	}

	/// <summary>
	/// Determines whether one or more bit fields are set in the current instance.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
	/// <param name="this">The current enumeration type instance.</param>
	/// <param name="other">The other instance to check.</param>
	/// <returns>
	/// <see langword="true"/> if the bit field or bit fields that are set in <paramref name="other"/>
	/// are also set in the current instance; otherwise, <see langword="false"/>.
	/// </returns>
	/// <exception cref="ArgumentException">Throws when the used bytes aren't 1, 2 or 4.</exception>
	public static bool Flags<TEnum>(this TEnum @this, TEnum other) where TEnum : unmanaged, Enum =>
		sizeof(TEnum) switch
		{
			1 or 2 or 4 when Unsafe.As<TEnum, int>(ref other) is var otherValue =>
				(Unsafe.As<TEnum, int>(ref @this) & otherValue) == otherValue,
			8 when Unsafe.As<TEnum, long>(ref other) is var otherValue =>
				(Unsafe.As<TEnum, long>(ref @this) & otherValue) == otherValue,
			_ => throw new ArgumentException("The parameter should be one of the values 1, 2, 4.", nameof(@this))
		};

	/// <summary>
	/// Determines whether the instance has the flags specified as <paramref name="flags"/>.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enumeration field.</typeparam>
	/// <param name="this">The instance.</param>
	/// <param name="flags">All flags used.</param>
	/// <returns>A <see cref="bool"/> result.</returns>
	public static bool MultiFlags<TEnum>(this TEnum @this, TEnum flags) where TEnum : unmanaged, Enum
	{
		foreach (var flag in flags)
		{
			if (@this.Flags(flag))
			{
				return true;
			}
		}

		return false;
	}
}
