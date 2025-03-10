namespace Sudoku.CommandLine;

/// <summary>
/// Represents a list of argument parsers.
/// </summary>
public sealed class ArgumentParserCollection : IEnumerable<ArgumentParser>
{
	/// <summary>
	/// Indicates the parsers.
	/// </summary>
	private readonly List<ArgumentParser> _parsers = [];

	/// <summary>
	/// Indicates the dictionary that stores a list of parsers and its required index.
	/// </summary>
	private readonly Dictionary<int, ArgumentParser> _positionParsers = [];


	/// <summary>
	/// Indicates the number of parsers.
	/// </summary>
	public int Count => _parsers.Count;


	/// <summary>
	/// Gets the parser that has set the required position.
	/// </summary>
	/// <param name="positionIndex">Indicates the position index.</param>
	/// <returns>
	/// The parser. If the specified position doesn't exist any parser,
	/// <see langword="null"/> will be returned without exception thrown.
	/// </returns>
	public ArgumentParser? this[int positionIndex] => _positionParsers.TryGetValue(positionIndex, out var value) ? value : null;


	/// <summary>
	/// Adds a new item into the collection.
	/// </summary>
	/// <param name="parser">The parsers.</param>
	public void Add(ArgumentParser parser) => _parsers.Add(parser);

	/// <summary>
	/// Sets the parser at the specified position.
	/// </summary>
	/// <param name="parser">The parser.</param>
	/// <param name="position">The position to be parsed.</param>
	/// <exception cref="InvalidOperationException">
	/// Throws when the specified position has already been set with another parser.
	/// </exception>
	public void SetPosition(ArgumentParser parser, int position)
	{
		if (_positionParsers.ContainsKey(position))
		{
			throw new InvalidOperationException("The specified position has already been set with another parser.");
		}
		_positionParsers.Add(position, parser);
	}

	/// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
	public AnonymousSpanEnumerator<ArgumentParser> GetEnumerator() => new(_parsers.AsSpan());

	/// <inheritdoc/>
	IEnumerator IEnumerable.GetEnumerator() => _parsers.GetEnumerator();

	/// <inheritdoc/>
	IEnumerator<ArgumentParser> IEnumerable<ArgumentParser>.GetEnumerator() => _parsers.GetEnumerator();
}
