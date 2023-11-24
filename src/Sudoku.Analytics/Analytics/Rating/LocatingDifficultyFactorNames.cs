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
}
