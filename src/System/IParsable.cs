namespace System;

/// <summary>
/// Defines a type that supports the custom parsing operations.
/// </summary>
/// <typeparam name="T">The type to parse.</typeparam>
public interface IParsable<T>
{
	/// <summary>
	/// Try to parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value shouldn't be <see langword="null"/>.</param>
	/// <param name="result">
	/// The result parsed. If failed to parse, the value will keep the <see langword="default"/> value,
	/// i.e. <see langword="default"/>(<typeparamref name="T"/>).
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the operation is successful to execute.
	/// </returns>
	static abstract bool TryParse(
		[NotNullWhen(true)] string? str,
		[NotNullWhen(true), DiscardWhen(false)] out T? result
	);

	/// <summary>
	/// Parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value shouln't be <see langword="null"/>.</param>
	/// <returns>The result parsed.</returns>
	/// <exception cref="FormatException">Throws when failed to parse.</exception>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="str"/> is <see langword="null"/>.
	/// </exception>
	[return: NotNullIfNotNull("str")]
	static abstract T? Parse(string? str);
}
