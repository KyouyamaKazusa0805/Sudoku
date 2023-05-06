namespace Sudoku.SourceGeneration.CollectedResults;

/// <summary>
/// Indicates the data collected via <see cref="EqualsOverriddenHandler"/>
/// </summary>
/// <seealso cref="EqualsOverriddenHandler"/>
internal sealed record DefaultOverriddenCollectedResult(
	EqualsOverriddenCollectedResult[] DataForEquals,
	GetHashCodeCollectedResult[] DataForGetHashCode,
	ToStringCollectedResult[] DataForToString
);
