namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Rectangles;

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
public record UniqueRectangleWithConjugatePairStep(
	in ImmutableArray<Conclusion> Conclusions,
	in ImmutableArray<PresentationData> Views,
	Technique TechniqueCode2,
	int Digit1,
	int Digit2,
	in Cells Cells,
	bool IsAvoidable,
	ConjugatePair[] ConjugatePairs,
	int AbsoluteOffset
) : UniqueRectangleStep(Conclusions, Views, TechniqueCode2, Digit1, Digit2, Cells, IsAvoidable, AbsoluteOffset)
{
	/// <inheritdoc/>
	public sealed override decimal Difficulty => 4.4M + ConjugatePairs.Length * .2M;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.UrPlus;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Often;

	/// <summary>
	/// Indicates the conjugate pair string.
	/// </summary>
	[FormatItem]
	protected string ConjPairsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			const string separator = ", ";

			var sb = new ValueStringBuilder(stackalloc char[100]);
			sb.AppendRange(ConjugatePairs, separator);

			return sb.ToStringAndClear();
		}
	}

	[FormatItem]
	private string Prefix
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePairs.Length == 1 ? "a " : string.Empty;
	}

	[FormatItem]
	private string Suffix
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ConjugatePairs.Length == 1 ? string.Empty : "s";
	}
}
