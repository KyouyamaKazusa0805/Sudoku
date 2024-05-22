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
	[PrimaryConstructorParameter] Mask nTimesAlmostLockedSetDigitsMask,
	[PrimaryConstructorParameter] ref readonly CellMap nTimesAlmostLockedSetCells,
	[PrimaryConstructorParameter] NTimesAlmostLockedSetsBlossomBranchCollection branches,
	[PrimaryConstructorParameter] int freedomDegree
) :
	DeathBlossomBaseStep(conclusions, views, options),
	IBranchTrait,
	IDeathBlossomCollection<NTimesAlmostLockedSetsBlossomBranchCollection, CandidateMap>
{
	/// <inheritdoc/>
	public override int BaseDifficulty => base.BaseDifficulty + 5;

	/// <inheritdoc/>
	public override Technique Code => Technique.NTimesAlmostLockedSetDeathBlossom;

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [FreedomDegreeStr, CellsStr, DigitsStr, BranchesStr]),
			new(ChineseLanguage, [FreedomDegreeStr, CellsStr, DigitsStr, BranchesStr])
		];

	/// <inheritdoc/>
	public override FactorCollection Factors => [new NTimesAlmostLockedSetsDeathBlossomPetalsCountFactor()];

	/// <inheritdoc/>
	int IBranchTrait.BranchesCount => Branches.Count;

	private string FreedomDegreeStr => FreedomDegree.ToString();

	private string CellsStr => Options.Converter.CellConverter(NTimesAlmostLockedSetCells);

	private string DigitsStr => Options.Converter.DigitConverter(NTimesAlmostLockedSetDigitsMask);

	private string BranchesStr
		=> string.Join(
			ResourceDictionary.Get("Comma", ResultCurrentCulture),
#if !NET9_0_OR_GREATER
			[
			..
#endif
			from branch in Branches
			let p = Options.Converter.CandidateConverter(branch.Key)
			let q = branch.AlsPattern.ToString(Options.Converter)
			select $"{p} - {q}"
#if !NET9_0_OR_GREATER
			]
#endif
		);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is NTimesAlmostLockedSetDeathBlossomStep comparer
		&& (NTimesAlmostLockedSetCells, NTimesAlmostLockedSetDigitsMask, Branches) == (comparer.NTimesAlmostLockedSetCells, comparer.NTimesAlmostLockedSetDigitsMask, comparer.Branches);

	/// <inheritdoc/>
	public override int CompareTo(Step? other)
	{
		if (other is not NTimesAlmostLockedSetDeathBlossomStep comparer)
		{
			return 1;
		}

		if (Branches.Count.CompareTo(comparer.Branches.Count) is var r1 and not 0)
		{
			return r1;
		}

		var leftCellsCount = Branches.Values.Sum(alsCellsCountSelector);
		var rightCellsCount = comparer.Branches.Values.Sum(alsCellsCountSelector);
		if (leftCellsCount.CompareTo(rightCellsCount) is var r2 and not 0)
		{
			return r2;
		}

		if (Conclusions.Length.CompareTo(comparer.Conclusions.Length) is var r3 and not 0)
		{
			return r3;
		}

		if (NTimesAlmostLockedSetCells.CompareTo(comparer.NTimesAlmostLockedSetCells) is var r4 and not 0)
		{
			return r4;
		}

		if (NTimesAlmostLockedSetDigitsMask.CompareTo(comparer.NTimesAlmostLockedSetDigitsMask) is var r5 and not 0)
		{
			return r5;
		}

		foreach (var branchCandidates in Branches.Keys)
		{
			if (Branches[branchCandidates].CompareTo(comparer.Branches[branchCandidates]) is var r6 and not 0)
			{
				return r6;
			}
		}

		return 0;


		static int alsCellsCountSelector(AlmostLockedSet s) => s.Cells.Count;
	}
}
