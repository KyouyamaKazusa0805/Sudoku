using Sudoku.Analytics;
using Sudoku.Concepts.Primitive;

namespace Sudoku.Concepts.Parsers;

/// <summary>
/// Represents for a parser instance that parses a <see cref="string"/> text,
/// converting it into a valid <see cref="ICoordinateObject"/> instance.
/// </summary>
/// <seealso cref="ICoordinateObject"/>
public abstract record CoordinateParser : GenericConceptParser
{
	/// <summary>
	/// The parser method that can creates a <see cref="CellMap"/> via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="CellMap"/>
	public abstract Func<string, CellMap> CellParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="CandidateMap"/> via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="CandidateMap"/>
	public abstract Func<string, CandidateMap> CandidateParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="HouseMask"/> via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="HouseMask"/>
	public abstract Func<string, HouseMask> HouseParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="Conclusion"/> list via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="Conclusion"/>
	public abstract Func<string, Conclusion[]> ConclusionParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="Mask"/> via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="Mask"/>
	public abstract Func<string, Mask> DigitParser { get; }

	/// <summary>
	/// The parser method that can creates a list of pairs of <see cref="IntersectionBase"/> and <see cref="IntersectionResult"/>
	/// via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="IntersectionBase"/>
	/// <seealso cref="IntersectionResult"/>
	public abstract Func<string, (IntersectionBase Base, IntersectionResult Result)[]> IntersectionParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="Chute"/> list via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="Chute"/>
	public abstract Func<string, Chute[]> ChuteParser { get; }

	/// <summary>
	/// The parser method that can creates a <see cref="Conjugate"/> list via the specified text to be parsed.
	/// </summary>
	/// <seealso cref="Conjugate"/>
	public abstract Func<string, Conjugate[]> ConjuagteParser { get; }
}
