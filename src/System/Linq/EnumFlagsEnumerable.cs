namespace System.Linq;

/// <summary>
/// Provides with some extension methods for enumeration types.
/// </summary>
public static class EnumFlagsEnumerable
{
	/// <summary>
	/// Try to get all <typeparamref name="TEnum"/> elements as flags stored in argument <paramref name="this"/>,
	/// and convert them into an array of <typeparamref name="T"/> instances via specified argument <paramref name="selector"/>,
	/// then return the array.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
	/// <typeparam name="T">The type of the result elements.</typeparam>
	/// <param name="this">The enumeration type field.</param>
	/// <param name="selector">The selector that can convert the enumeration field into the target-typed instance.</param>
	/// <returns>An array of <typeparamref name="T"/> elements.</returns>
	/// <exception cref="InvalidOperationException">
	/// Throws when the enumeration type is not marked <see cref="FlagsAttribute"/>.
	/// </exception>
	public static T[] Select<TEnum, T>(this TEnum @this, Func<TEnum, T> selector) where TEnum : unmanaged, Enum
	{
		var flags = @this.GetAllFlags() ?? throw new InvalidOperationException($"The enumeration type must be marked as '{nameof(FlagsAttribute)}'.");
		var result = new T[flags.Length];
		for (var i = 0; i < flags.Length; i++)
		{
			result[i] = selector(flags[i]);
		}

		return result;
	}
}
