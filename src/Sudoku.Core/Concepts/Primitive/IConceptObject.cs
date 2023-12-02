using System.Diagnostics.CodeAnalysis;
using Sudoku.Text;

namespace Sudoku.Concepts.Primitive;

/// <summary>
/// Represents an object type that describes for an object that is defined in sudoku concepts.
/// </summary>
/// <typeparam name="TSelf">The type of implementation.</typeparam>
/// <typeparam name="TConverter">The type of the converter.</typeparam>
/// <typeparam name="TParser">The type of the parser.</typeparam>
public interface IConceptObject<TSelf, TConverter, TParser>
	where TSelf : IConceptObject<TSelf, TConverter, TParser>
	where TConverter : ISpecifiedConceptConverter<TSelf>
	where TParser : ISpecifiedConceptParser<TSelf>
{
	/// <summary>
	/// Try to convert the current instance into an equivalent <see cref="string"/> representation,
	/// using the specified formatting rule defined in argument <paramref name="converter"/>.
	/// </summary>
	/// <param name="converter">A converter instance that defines the conversion rule.</param>
	/// <returns>The target <see cref="string"/> representation.</returns>
	public abstract string ToString(TConverter converter);


	/// <summary>
	/// Parses the specified <see cref="string"/> text and convert into a <typeparamref name="TSelf"/> instance,
	/// using the specified parsing rule.
	/// </summary>
	/// <param name="str">The string text to be parsed.</param>
	/// <param name="parser">The parser instance to be used.</param>
	/// <returns>A valid <typeparamref name="TSelf"/> instance parsed.</returns>
	/// <exception cref="FormatException">Throws when the target <typeparamref name="TParser"/> instance cannot parse it.</exception>
	public static abstract TSelf ParseExact(string str, TParser parser);

	/// <summary>
	/// Try to parse the specified <see cref="string"/> text and convert into a <typeparamref name="TSelf"/> instance,
	/// using the specified parsing rule. If the parsing operation is failed, return <see langword="false"/> to report the failure case.
	/// No exceptions will be thrown.
	/// </summary>
	/// <param name="str">The string text to be parsed.</param>
	/// <param name="parser">The parser instance to be used.</param>
	/// <param name="result">A parsed value of type <typeparamref name="TSelf"/>.</param>
	/// <returns>Indicates whether the parsing operation is successful.</returns>
	public static sealed bool TryParseExact(string str, TParser parser, [NotNullWhen(true)] out TSelf? result)
	{
		try
		{
			result = parser.Parser(str);
			return true;
		}
		catch (FormatException)
		{
			result = default;
			return false;
		}
	}
}
