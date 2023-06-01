namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Naked Subset</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="house"><inheritdoc/></param>
/// <param name="cells"><inheritdoc/></param>
/// <param name="digitsMask"><inheritdoc/></param>
/// <param name="isLocked">
/// Indicates which locked type this subset is. The cases are as belows:
/// <list type="table">
/// <item>
/// <term><see langword="true"/></term>
/// <description>The subset is a locked subset.</description>
/// </item>
/// <item>
/// <term><see langword="false"/></term>
/// <description>The subset is a naked subset with at least one extra locked candidate.</description>
/// </item>
/// <item>
/// <term><see langword="null"/></term>
/// <description>The subset is a normal naked subset without any extra locked candidates.</description>
/// </item>
/// </list>
/// </param>
public sealed partial class NakedSubsetStep(
	Conclusion[] conclusions,
	View[]? views,
	House house,
	scoped in CellMap cells,
	Mask digitsMask,
	[PrimaryConstructorParameter] bool? isLocked
) : SubsetStep(conclusions, views, house, cells, digitsMask)
{
	/// <inheritdoc/>
	public override Technique Code
		=> (IsLocked, Size) switch
		{
			(true, 2) => Technique.LockedPair,
			(false, 2) => Technique.NakedPairPlus,
			(_, 2) => Technique.NakedPair,
			(true, 3) => Technique.LockedTriple,
			(false, 3) => Technique.NakedTriplePlus,
			(_, 3) => Technique.NakedTriple,
			(false, 4) => Technique.NakedQuadruplePlus,
			(null, 4) => Technique.NakedQuadruple
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
