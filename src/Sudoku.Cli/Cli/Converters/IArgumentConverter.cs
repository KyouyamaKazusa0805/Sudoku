namespace Sudoku.Cli.Converters;

/// <summary>
/// Represents a type that supports converting from the <see cref="string"/> raw argument to the specified type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="TSelf">The type of the converter type itself.</typeparam>
/// <typeparam name="T">The type of the final converted value.</typeparam>
public interface IArgumentConverter<TSelf, out T> where TSelf : IArgumentConverter<TSelf, T>
{
	/// <summary>
	/// Converts the specified argument raw data (tokens and error handler) into the target instance of type <typeparamref name="T"/>.
	/// </summary>
	/// <param name="argumentResult">The argument instance.</param>
	/// <returns>The result instance having parsed.</returns>
	static abstract T ConvertValue(ArgumentResult argumentResult);
}
