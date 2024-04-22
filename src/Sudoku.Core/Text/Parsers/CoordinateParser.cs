namespace Sudoku.Text.Parsers;

/// <summary>
/// Represents for a parser instance that parses a <see cref="string"/> text,
/// converting it into a valid <see cref="ICoordinateObject{TSelf}"/> instance.
/// </summary>
/// <seealso cref="ICoordinateObject{TSelf}"/>
public abstract record CoordinateParser : GenericConceptParser
{
	/// <summary>
	/// The not supported information for property implemented.
	/// </summary>
	protected const string DeprecatedInfo_NotSupported = "This property is not implemented due to the lack of K9 notation rules.";


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
	public abstract Func<string, ConclusionSet> ConclusionParser { get; }

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
	public abstract Func<string, Intersection[]> IntersectionParser { get; }

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
