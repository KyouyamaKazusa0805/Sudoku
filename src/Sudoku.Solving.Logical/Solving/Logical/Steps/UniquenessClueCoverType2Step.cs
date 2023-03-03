namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Clue Cover Type 2</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1">Indicates the first digit.</param>
/// <param name="Digit2">Indicates the second digit.</param>
/// <param name="Cell1">Indicates the first cell used.</param>
/// <param name="Cell2">Indicates the second cell used.</param>
/// <param name="ChuteIndex">Indicates the global chute index.</param>
internal sealed record UniquenessClueCoverType2Step(
	Conclusion[] Conclusions,
	View[]? Views,
	int Digit1,
	int Digit2,
	int Cell1,
	int Cell2,
	int ChuteIndex
) : UniquenessClueCoverStep(Conclusions, Views, CellsMap[Cell1] + Cell2)
{
	/// <inheritdoc/>
	public override int Type => 2;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.OnlyForSpecialPuzzles;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { ChuteIndexStr, HousesStr } }, { "zh", new[] { ChuteIndexStr, HousesStr } } };

	private string ChuteIndexStr => (ChuteIndex + 1).ToString();

	private string HousesStr
	{
		get
		{
			var (h1, h2, h3) = ChuteHouses[ChuteIndex];
			return HouseFormatter.Format(1 << h1 | 1 << h2 | 1 << h3);
		}
	}
}
