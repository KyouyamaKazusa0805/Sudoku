namespace Sudoku.Solving.Logical.Steps;

/// <summary>
/// Provides with a step that is a <b>Subset</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="House">The house that structure lies in.</param>
/// <param name="Cells">All cells used.</param>
/// <param name="DigitsMask">The mask that contains all digits used.</param>
internal abstract record SubsetStep(Conclusion[] Conclusions, View[]? Views, int House, scoped in CellMap Cells, short DigitsMask) :
	Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.0M;

	/// <summary>
	/// Indicates the number of cells used.
	/// Due to the technique logic, you can also treat the result value of this property as the number of digits used.
	/// </summary>
	public int Size => PopCount((uint)DigitsMask);

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Moderate;

	/// <inheritdoc/>
	public sealed override TechniqueTags TechniqueTags => TechniqueTags.Subsets;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.Subset;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => (Rarity)(Size - 1 << 1);
}
