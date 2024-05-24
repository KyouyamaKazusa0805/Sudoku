namespace Sudoku.Concepts.Coordinates;

/// <summary>
/// Represents an interface type that describes for an ability on <c>Parse</c> with custom sudoku coordinate logic.
/// </summary>
/// <typeparam name="TSelf">The type of itself.</typeparam>
public interface ICoordinateParsable<TSelf> : IParsable<TSelf> where TSelf : ICoordinateParsable<TSelf>
{
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

	/// <summary>
	/// Try to parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <typeparam name="T">The type of coordinate parser.</typeparam>
	/// <param name="str">The string to parse. The value cannot be <see langword="null"/>.</param>
	/// <param name="parser">
	/// A parser instance derived from <see cref="CoordinateParser"/>,
	/// indicating the custom logic on parsing for coordinate items inside the concept.
	/// </param>
	/// <param name="result">
	/// The result parsed. If failed to parse, the value will keep the <see langword="default"/> value,
	/// i.e. <see langword="default"/>(<typeparamref name="TSelf"/>).
	/// </param>
	/// <returns>
	/// A <see cref="bool"/> result indicating whether the operation is successful to execute.
	/// </returns>
	public static abstract bool TryParse<T>(string str, T parser, [NotNullWhen(true)] out TSelf? result) where T : CoordinateParser;

	/// <summary>
	/// Parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <param name="str">The string to parse. The value cannot be <see langword="null"/>.</param>
	/// <returns>The result parsed.</returns>
	/// <exception cref="FormatException">Throws when failed to parse.</exception>
	public static abstract TSelf Parse(string str);

	/// <summary>
	/// Parse the specified string text, and get the same-meaning instance
	/// of type <typeparamref name="TSelf"/>.
	/// </summary>
	/// <typeparam name="T">The type of coordinate parser.</typeparam>
	/// <param name="str">The string to parse. The value cannot be <see langword="null"/>.</param>
	/// <param name="parser">
	/// A parser instance derived from <see cref="CoordinateParser"/>,
	/// indicating the custom logic on parsing for coordinate items inside the concept.
	/// </param>
	/// <returns>The result parsed.</returns>
	/// <exception cref="FormatException">Throws when failed to parse.</exception>
	public static abstract TSelf Parse<T>(string str, T parser) where T : CoordinateParser;
}
