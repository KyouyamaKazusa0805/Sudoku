namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>n-Times ALS Death Blossom</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="nTimesAlmostLockedSetsDigitsMask">Indicates the digits A^nLS used.</param>
/// <param name="nTimesAlmostLockedSetsCells">Indicates the A^nLS cells used.</param>
/// <param name="branches">Indicates the detail branches.</param>
/// <param name="freedomDegree">Indicates the freedom degree of this A^nLS.</param>
public sealed partial class NTimesAlmostLockedSetsDeathBlossomStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Mask nTimesAlmostLockedSetsDigitsMask,
	[PrimaryConstructorParameter] ref readonly CellMap nTimesAlmostLockedSetsCells,
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
	public override Technique Code => Technique.NTimesAlmostLockedSetsDeathBlossom;

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

	private string CellsStr => Options.Converter.CellConverter(NTimesAlmostLockedSetsCells);

	private string DigitsStr => Options.Converter.DigitConverter(NTimesAlmostLockedSetsDigitsMask);

	private string BranchesStr
		=> string.Join(
			SR.Get("Comma", GetCulture(null)),
			from branch in Branches
			let p = Options.Converter.CandidateConverter(branch.Key)
			let q = branch.Value.ToString(Options.Converter)
			select $"{p} - {q}"
		);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is NTimesAlmostLockedSetsDeathBlossomStep comparer
		&& (NTimesAlmostLockedSetsCells, NTimesAlmostLockedSetsDigitsMask, Branches) == (comparer.NTimesAlmostLockedSetsCells, comparer.NTimesAlmostLockedSetsDigitsMask, comparer.Branches);

	/// <inheritdoc/>
	public override int CompareTo(Step? other)
	{
		if (other is not NTimesAlmostLockedSetsDeathBlossomStep comparer)
		{
			return -1;
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

		if (NTimesAlmostLockedSetsCells.CompareTo(comparer.NTimesAlmostLockedSetsCells) is var r4 and not 0)
		{
			return r4;
		}

		if (NTimesAlmostLockedSetsDigitsMask.CompareTo(comparer.NTimesAlmostLockedSetsDigitsMask) is var r5 and not 0)
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
