namespace System;

/// <summary>
/// Defines an instance that allows the <see cref="string"/> value
/// to be parsed to the target type <typeparamref name="TSelf"/>.
/// </summary>
/// <remarks>
/// Different with type <see cref="IParsable{TSelf}"/>, this type is not necessary to convert
/// instances into <see cref="IFormatProvider"/>.
/// </remarks>
/// <typeparam name="TSelf">The type of the target result.</typeparam>
/// <seealso cref="IParsable{TSelf}"/>
/// <seealso cref="IFormatProvider"/>
public interface ISimpleParsable<TSelf> : IParsable<TSelf> where TSelf : ISimpleParsable<TSelf>
{
	/// <summary>
	/// Parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value cannot be <see langword="null"/>.</param>
	/// <returns>The result parsed.</returns>
	/// <exception cref="FormatException">Throws when failed to parse.</exception>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="str"/> is <see langword="null"/>.
	/// </exception>
	public static abstract TSelf Parse(string str);

	/// <summary>
	/// Try to parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value cannot be <see langword="null"/>.</param>
	/// <param name="result">
	/// The result parsed. If failed to parse, the value will keep the <see langword="default"/> value,
	/// i.e. <see langword="default"/>(<typeparamref name="TSelf"/>).
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the operation is successful to execute.
	/// </returns>
	public static abstract bool TryParse(string str, [NotNullWhen(true)] out TSelf? result);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IParsable<TSelf>.TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [NotNullWhen(true)] out TSelf? result)
	{
		if (s is null)
		{
			result = default;
			return false;
		}
		else
		{
			return TSelf.TryParse(s, out result);
		}
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static TSelf IParsable<TSelf>.Parse(string s, IFormatProvider? provider) => TSelf.Parse(s);
}
