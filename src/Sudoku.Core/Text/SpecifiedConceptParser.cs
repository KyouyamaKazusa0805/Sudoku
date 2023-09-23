namespace Sudoku.Text;

/// <summary>
/// Represents a parser that only parses into a specified concept object from a specified <see cref="string"/> text.
/// </summary>
/// <typeparam name="T">The type of return value.</typeparam>
public abstract record SpecifiedConceptParser<T> : ConceptParser
{
	/// <summary>
	/// A parser instance that parses the specified <see cref="string"/> text, converting it into a valid <typeparamref name="T"/> instance.
	/// </summary>
	public abstract Func<string, T> Parser { get; }
}
