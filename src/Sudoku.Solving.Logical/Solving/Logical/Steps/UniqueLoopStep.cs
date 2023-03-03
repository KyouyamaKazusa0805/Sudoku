namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit1">Indicates the first digit.</param>
/// <param name="Digit2">Indicates the second digit.</param>
/// <param name="Loop">Indicates the loop that the instance used.</param>
internal abstract record UniqueLoopStep(Conclusion[] Conclusions, View[]? Views, int Digit1, int Digit2, scoped in CellMap Loop) :
	DeadlyPatternStep(Conclusions, Views),
	IDistinctableStep<UniqueLoopStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the type.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode => Enum.Parse<Technique>($"UniqueLoopType{Type}");

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.UniqueLoop;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.DeadlyPattern;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Length, (A004526(Loop.Count) - 3) * .1M) };

	private protected string LoopStr => Loop.ToString();

	private protected string Digit1Str => (Digit1 + 1).ToString();

	private protected string Digit2Str => (Digit2 + 1).ToString();


	/// <inheritdoc/>
	public static bool Equals(UniqueLoopStep left, UniqueLoopStep right)
		=> left.Type == right.Type && left.Loop == right.Loop
		&& (1 << left.Digit1 | 1 << left.Digit2) == (1 << right.Digit1 | 1 << right.Digit2)
		&& (left, right) switch
		{
			(UniqueLoopType3Step { SubsetDigitsMask: var a }, UniqueLoopType3Step { SubsetDigitsMask: var b }) => a == b,
			(UniqueLoopType4Step { ConjugatePair: var a }, UniqueLoopType4Step { ConjugatePair: var b }) => a == b,
			_ => true
		};
}
