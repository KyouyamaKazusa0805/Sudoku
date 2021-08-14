namespace System;

/// <summary>
/// Defines a parsable type.
/// </summary>
/// <typeparam name="T">The type to parse.</typeparam>
[RequiresPreviewFeatures]
public interface IParsable<T>
{
	/// <summary>
	/// Try to parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="str">The string to parse.</param>
	/// <param name="result">
	/// The result parsed. If failed to parse, the value will keep the <see langword="default"/> value,
	/// i.e. <see langword="default"/>(<typeparamref name="T"/>).
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the operation is successful to execute.
	/// </returns>
	static abstract bool TryParse(string str, out T result);

	/// <summary>
	/// Parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="str">The string to parse.</param>
	/// <returns>The result parsed.</returns>
	/// <exception cref="FormatException">Throws when failed to parse.</exception>
	static abstract T Parse(string str);
}
