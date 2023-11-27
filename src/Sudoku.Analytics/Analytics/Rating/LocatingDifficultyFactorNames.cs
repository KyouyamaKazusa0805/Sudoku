namespace Sudoku.Analytics.Rating;

/// <summary>
/// Represents with kinds of the phased difficulty rating that is used for the calculations the total rating of a step on locating.
/// </summary>
public static class LocatingDifficultyFactorNames
{
	/// <summary>
	/// Indicates the factor is house type.
	/// </summary>
	public const string HouseType = nameof(HouseType);

	/// <summary>
	/// Indicates the factor is house position.
	/// </summary>
	public const string HousePosition = nameof(HousePosition);

	/// <summary>
	/// Indicates the factor is digit.
	/// </summary>
	public const string Digit = nameof(Digit);

	/// <summary>
	/// Indicates the factor is hidden single excluder.
	/// </summary>
	public const string HiddenSingleExcluder = nameof(HiddenSingleExcluder);

	/// <summary>
	/// Indicates the factor is naked single excluder.
	/// </summary>
	public const string NakedSingleExcluder = nameof(NakedSingleExcluder);

	/// <summary>
	/// Indicates the factor is locked candidates cells count.
	/// </summary>
	public const string LockedCandidatesCellsCount = nameof(LockedCandidatesCellsCount);

	/// <summary>
	/// Indicates the factor is size.
	/// </summary>
	public const string Size = nameof(Size);

	/// <summary>
	/// Indicates the factor is distance.
	/// </summary>
	public const string Distance = nameof(Distance);

	/// <summary>
	/// Indicates the factor is interferer.
	/// </summary>
	public const string Interferer = nameof(Interferer);

	/// <summary>
	/// Indicates the factor is fin.
	/// </summary>
	public const string Fin = nameof(Fin);

	/// <summary>
	/// Indicates the factor is incompleteness.
	/// </summary>
	public const string Incompleteness = nameof(Incompleteness);

	/// <summary>
	/// Indicates the factor is sashimi.
	/// </summary>
	public const string Sashimi = nameof(Sashimi);

	/// <summary>
	/// Indicates the factor is petals.
	/// </summary>
	public const string Petals = nameof(Petals);

	/// <summary>
	/// Indicates the factor is connecter.
	/// </summary>
	public const string Connector = nameof(Connector);

	/// <summary>
	/// Indicates the factor is avoidable rectangle.
	/// </summary>
	public const string AvoidableRectangle = nameof(AvoidableRectangle);

	/// <summary>
	/// Indicates the factor is extra digit.
	/// </summary>
	public const string ExtraDigit = nameof(ExtraDigit);

	/// <summary>
	/// Indicates the factor is conjugate pair.
	/// </summary>
	public const string ConjugatePair = nameof(ConjugatePair);

	/// <summary>
	/// Indicates the factor is empty cell.
	/// </summary>
	public const string EmptyCell = nameof(EmptyCell);

	/// <summary>
	/// Indicates the factor is true candidate.
	/// </summary>
	public const string TrueCandidate = nameof(TrueCandidate);

	/// <summary>
	/// Indicates the factor is digit variance.
	/// </summary>
	public const string DigitVariance = nameof(DigitVariance);

	/// <summary>
	/// Indicates the factor is empty rectangle cells count.
	/// </summary>
	public const string EmptyRectangleCellsCount = nameof(EmptyRectangleCellsCount);
}
