namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Naked Subset</b> technique.
/// </summary>
public sealed class NakedSubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	int house,
	scoped in CellMap cells,
	Mask digitsMask,
	bool? isLocked
) : SubsetStep(conclusions, views, house, cells, digitsMask)
{
	/// <summary>
	/// Indicates which locked type this subset is. The cases are as belows:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The subset is a locked subset.</description>
	/// </item>
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The subset is a naked subset with at least one extra locked candidate.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The subset is a normal naked subset without any extra locked candidates.</description>
	/// </item>
	/// </list>
	/// </summary>
	public bool? IsLocked { get; } = isLocked;

	/// <inheritdoc/>
	public override Technique Code
		=> (IsLocked, Size) switch
		{
			(true, 2) => Technique.LockedPair,
			(false, 2) => Technique.NakedPairPlus,
			(null, 2) => Technique.NakedPair,
			(true, 3) => Technique.LockedTriple,
			(false, 3) => Technique.NakedTriplePlus,
			(null, 3) => Technique.NakedTriple,
			(false, 4) => Technique.NakedQuadruplePlus,
			(null, 4) => Technique.NakedQuadruple,
		};

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[]
		{
			(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => .6M, 4 => 2.0M }),
			(ExtraDifficultyCaseNames.Locked, IsLocked switch { true => Size switch { 2 => -1.0M, 3 => -1.1M }, false => .1M, _ => 0 })
		};

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?> { { "en", new[] { DigitsStr, HouseStr } }, { "zh", new[] { DigitsStr, HouseStr, SubsetName } } };

	private string DigitsStr => DigitMaskFormatter.Format(DigitsMask, FormattingMode.Normal);

	private string HouseStr => HouseFormatter.Format(1 << House);

	private string SubsetName => R[$"SubsetNamesSize{Size}"]!;
}
