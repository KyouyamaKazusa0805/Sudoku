using Sudoku.Concepts.Primitive;

namespace Sudoku.Concepts.Parsers;

/// <summary>
/// Represents a parser instance.
/// </summary>
public abstract record GridParser : ISpecifiedConceptParser<Grid>
{
	/// <inheritdoc/>
	public abstract Func<string, Grid> Parser { get; }
}
