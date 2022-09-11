namespace Sudoku.Solving.Logics.Implementations.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle with Conjugate Pair(s)</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="TechniqueCode2"><inheritdoc/></param>
/// <param name="Digit1"><inheritdoc/></param>
/// <param name="Digit2"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="IsAvoidable"><inheritdoc/></param>
/// <param name="ConjugatePairs">Indicates the conjugate pairs used.</param>
/// <param name="AbsoluteOffset"><inheritdoc/></param>
internal record UniqueRectangleWithConjugatePairStep(
	ConclusionList Conclusions,
	ViewList Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	scoped in CellMap Cells,
	bool IsAvoidable,
	Conjugate[] ConjugatePairs,
	int AbsoluteOffset
) :
	UniqueRectangleStep(
		Conclusions,
		Views,
		TechniqueCode2,
		Digit1,
		Digit2,
		Cells,
		IsAvoidable,
		AbsoluteOffset
	),
	IStepWithPhasedDifficulty
{
	/// <inheritdoc/>
	public sealed override decimal Difficulty => ((IStepWithPhasedDifficulty)this).TotalDifficulty;

	/// <inheritdoc/>
	public decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public (string Name, decimal Value)[] ExtraDifficultyValues
		=> new[] { (PhasedDifficultyRatingKinds.ConjugatePair, ConjugatePairs.Length * .2M) };

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueRectanglePlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	/// <summary>
	/// Indicates the conjugate pair string.
	/// </summary>
	[ResourceTextFormatter]
	internal string ConjPairsStr()
	{
		const string separator = ", ";

		scoped var sb = new StringHandler(100);
		sb.AppendRangeWithSeparator(ConjugatePairs, separator);

		return sb.ToStringAndClear();
	}

	[ResourceTextFormatter]
	internal string Prefix() => ConjugatePairs.Length == 1 ? "a " : string.Empty;

	[ResourceTextFormatter]
	internal string Suffix() => ConjugatePairs.Length == 1 ? string.Empty : "s";
}
