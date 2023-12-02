using System.Diagnostics.CodeAnalysis;
using Sudoku.Text.Converters;
using Sudoku.Text.Parsers;

namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Defines a type that supports for coordinate output rule.
/// </summary>
/// <typeparam name="TSelf">The type of implementation.</typeparam>
public interface ICoordinateObject<TSelf> where TSelf : ICoordinateObject<TSelf>
{
	/// <summary>
	/// Formats the current instance, converting it into a <see cref="string"/> result that can describe for this object.
	/// The conversion rule is specified as parameter <paramref name="converter"/>.
	/// </summary>
	/// <param name="converter">The coordinate converter object.</param>
	/// <returns>A <see cref="string"/> representation of the current object.</returns>
	public abstract string ToString(CoordinateConverter converter);


	/// <summary>
	/// Parses the specified <see cref="string"/> text and convert into a <typeparamref name="TSelf"/> instance,
	/// using the specified parsing rule.
	/// </summary>
	/// <param name="str">The string text to be parsed.</param>
	/// <param name="parser">The parser instance to be used.</param>
	/// <returns>A valid <typeparamref name="TSelf"/> instance parsed.</returns>
	/// <exception cref="FormatException">Throws when the <see cref="CoordinateParser"/> instance cannot parse it.</exception>
	public static abstract TSelf ParseExact(string str, CoordinateParser parser);

	/// <summary>
	/// Try to parse the specified <see cref="string"/> text and convert into a <typeparamref name="TSelf"/> instance,
	/// using the specified parsing rule. If the parsing operation is failed, return <see langword="false"/> to report the failure case.
	/// No exceptions will be thrown.
	/// </summary>
	/// <param name="str">The string text to be parsed.</param>
	/// <param name="parser">The parser instance to be used.</param>
	/// <param name="result">A parsed value of type <typeparamref name="TSelf"/>.</param>
	/// <returns>Indicates whether the parsing operation is successful.</returns>
	public static virtual bool TryParseExact(string str, CoordinateParser parser, [NotNullWhen(true)] out TSelf? result)
	{
		try
		{
			result = TSelf.ParseExact(str, parser);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}
}
