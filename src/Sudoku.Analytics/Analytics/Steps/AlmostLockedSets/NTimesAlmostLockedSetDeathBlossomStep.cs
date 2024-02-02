namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>n-Times ALS Death Blossom</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="nTimesAlmostLockedSetDigitsMask">Indicates the digits A^nLS used.</param>
/// <param name="nTimesAlmostLockedSetCells">Indicates the A^nLS cells used.</param>
/// <param name="branches">Indicates the detail branches.</param>
/// <param name="freedomDegree">Indicates the freedom degree of this A^nLS.</param>
public sealed partial class NTimesAlmostLockedSetDeathBlossomStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryCosntructorParameter] Mask nTimesAlmostLockedSetDigitsMask,
	[PrimaryCosntructorParameter] scoped ref readonly CellMap nTimesAlmostLockedSetCells,
	[PrimaryCosntructorParameter] NTimesAlmostLockedSetsBlossomBranchCollection branches,
	[PrimaryCosntructorParameter] int freedomDegree
) :
	DeathBlossomBaseStep(conclusions, views, options),
	IComparableStep<NTimesAlmostLockedSetDeathBlossomStep>,
	IEquatableStep<NTimesAlmostLockedSetDeathBlossomStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => base.BaseDifficulty + .5M;

	/// <inheritdoc/>
	public override Technique Code => Technique.NTimesAlmostLockedSetDeathBlossom;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [FreedomDegreeStr, CellsStr, DigitsStr, BranchesStr]),
			new(ChineseLanguage, [FreedomDegreeStr, CellsStr, DigitsStr, BranchesStr])
		];

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.Petals, A002024(Branches.Count) * .1M)];

	private string FreedomDegreeStr => FreedomDegree.ToString();

	private string CellsStr => Options.Converter.CellConverter(NTimesAlmostLockedSetCells);

	private string DigitsStr => Options.Converter.DigitConverter(NTimesAlmostLockedSetDigitsMask);

	private string BranchesStr
		=> string.Join(
			ResourceDictionary.Get("Comma", ResultCurrentCulture),
			[.. from branch in Branches select $"{Options.Converter.CandidateConverter(branch.Key)} - {branch.AlsPattern}"]
		);


	/// <inheritdoc/>
	static int IComparableStep<NTimesAlmostLockedSetDeathBlossomStep>.Compare(NTimesAlmostLockedSetDeathBlossomStep left, NTimesAlmostLockedSetDeathBlossomStep right)
	{
		if (left.Branches.Count.CompareTo(right.Branches.Count) is var comparisonResult1 and not 0)
		{
			return comparisonResult1;
		}

		var leftCellsCount = left.Branches.Values.Sum(alsCellsCountSelector);
		var rightCellsCount = right.Branches.Values.Sum(alsCellsCountSelector);
		if (leftCellsCount.CompareTo(rightCellsCount) is var comparisonResult2 and not 0)
		{
			return comparisonResult2;
		}

		if (left.Conclusions.Length.CompareTo(right.Conclusions.Length) is var comparisonResult3 and not 0)
		{
			return comparisonResult3;
		}

		if (left.NTimesAlmostLockedSetCells.CompareTo(right.NTimesAlmostLockedSetCells) is var comparisonResult4 and not 0)
		{
			return comparisonResult4;
		}

		if (left.NTimesAlmostLockedSetDigitsMask.CompareTo(right.NTimesAlmostLockedSetDigitsMask) is var comparisonResult5 and not 0)
		{
			return comparisonResult5;
		}

		foreach (var branchCandidates in left.Branches.Keys)
		{
			if (left.Branches[branchCandidates].CompareTo(right.Branches[branchCandidates]) is var comparisonResult6 and not 0)
			{
				return comparisonResult6;
			}
		}

		return 0;


		static int alsCellsCountSelector(AlmostLockedSet s) => s.Cells.Count;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<NTimesAlmostLockedSetDeathBlossomStep>.operator ==(NTimesAlmostLockedSetDeathBlossomStep left, NTimesAlmostLockedSetDeathBlossomStep right)
		=> (left.NTimesAlmostLockedSetCells, left.NTimesAlmostLockedSetDigitsMask, left.Branches)
		== (right.NTimesAlmostLockedSetCells, right.NTimesAlmostLockedSetDigitsMask, right.Branches);
}
