namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Death Blossom Cell Type</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="HubCell">Indicates a cell as the hub of petals.</param>
/// <param name="DigitsMask">The digits used.</param>
/// <param name="Petals">Indicates the petals used.</param>
internal sealed record DeathBlossomCellTypeStep(
	Conclusion[] Conclusions,
	View[]? Views,
	int HubCell,
	short DigitsMask,
	AlmostLockedSet[] Petals
) : DeathBlossomStep(Conclusions, Views, DigitsMask, Petals)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 8.3M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.DeathBlossomCellType;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[]
		{
			new(ExtraDifficultyCaseNames.Petals, Petals.Length switch { >= 2 and < 5 => .1M, >= 5 and < 7 => .2M, _ => .3M })
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { CellStr, AlsesStr, DigitStr } },
			{ "zh", new[] { CellStr, AlsesStr, DigitStr } }
		};

	private string CellStr => RxCyNotation.ToCellString(HubCell);
}
