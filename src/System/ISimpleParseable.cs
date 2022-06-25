namespace System;

/// <summary>
/// Defines an instance that allows the <see cref="string"/> value
/// to be parsed to the target type <typeparamref name="TSimpleParseable"/>.
/// </summary>
/// <typeparam name="TSimpleParseable">The type of the target result.</typeparam>
public interface ISimpleParseable</*[Self]*/ TSimpleParseable> where TSimpleParseable : ISimpleParseable<TSimpleParseable>
{
	/// <summary>
	/// Parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="TSimpleParseable"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value cannot be <see langword="null"/>.</param>
	/// <returns>The result parsed.</returns>
	/// <exception cref="FormatException">Throws when failed to parse.</exception>
	/// <exception cref="ArgumentNullException">
	/// Throws when the argument <paramref name="str"/> is <see langword="null"/>.
	/// </exception>
	public static abstract TSimpleParseable Parse(string str);

	/// <summary>
	/// Try to parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="TSimpleParseable"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value cannot be <see langword="null"/>.</param>
	/// <param name="result">
	/// The result parsed. If failed to parse, the value will keep the <see langword="default"/> value,
	/// i.e. <see langword="default"/>(<typeparamref name="TSimpleParseable"/>).
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the operation is successful to execute.
	/// </returns>
	public static abstract bool TryParse(string str, [NotNullWhen(true)] out TSimpleParseable? result);
}
