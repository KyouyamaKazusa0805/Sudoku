namespace System;

/// <summary>
/// Defines an instance that allows the <see cref="string"/> value
/// to be parsed to the target type <typeparamref name="TSelf"/>.
/// </summary>
/// <typeparam name="TSelf">The type of the target result.</typeparam>
public interface ISimpleParseable<TSelf> where TSelf : ISimpleParseable<TSelf>
{
	/// <summary>
	/// Parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value shouln't be <see langword="null"/>.</param>
	/// <returns>The result parsed.</returns>
	/// <exception cref="FormatException">Throws when failed to parse.</exception>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="str"/> is <see langword="null"/>.
	/// </exception>
	static abstract TSelf Parse(string? str);

	/// <summary>
	/// Try to parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value shouldn't be <see langword="null"/>.</param>
	/// <param name="result">
	/// The result parsed. If failed to parse, the value will keep the <see langword="default"/> value,
	/// i.e. <see langword="default"/>(<typeparamref name="TSelf"/>).
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the operation is successful to execute.
	/// </returns>
	static abstract bool TryParse(
		[NotNullWhen(true)] string? str,
		[NotNullWhen(true)] out TSelf? result
	);
}
