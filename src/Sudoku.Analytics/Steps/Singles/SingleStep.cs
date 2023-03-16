namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Single</b> technique.
/// </summary>
public abstract class SingleStep(Conclusion[] conclusions, View[]? views, int cell, int digit) : Step(conclusions, views)
{
	/// <summary>
	/// Indicates the cell used.
	/// </summary>
	public int Cell { get; } = cell;

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit { get; } = digit;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Easy;

	/// <inheritdoc/>
	public sealed override TechniqueGroup Group => TechniqueGroup.Single;
}
