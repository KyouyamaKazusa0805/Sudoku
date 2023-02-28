namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Clue Cover</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="ExtraCells">Indicates the extra cells used.</param>
internal abstract record UniquenessClueCoverStep(ConclusionList Conclusions, ViewList Views, scoped in CellMap ExtraCells) :
	DeadlyPatternStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 6.5M;

	/// <summary>
	/// Indicates the type.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override Technique TechniqueCode => Enum.Parse<Technique>($"UniquenessClueCoverType{Type}");

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.UniquenessClueCover;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.DeadlyPattern;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <inheritdoc/>
	public sealed override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, A004526(ExtraCells.Count) * .1M) };
}
