using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps.SingleDigitPatterns;

/// <summary>
/// Provides with a step that is a <b>Single Digit Pattern</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Digit">Indicates the digit used.</param>
public abstract record SingleDigitPatternStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Digit
) : Step(Conclusions, Views)
{
	/// <inheritdoc/>
	public sealed override bool ShowDifficulty => base.ShowDifficulty;

	/// <inheritdoc/>
	public sealed override bool IsElementary => base.IsElementary;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;

	/// <inheritdoc/>
	public override TechniqueTags TechniqueTags => TechniqueTags.SingleDigitPattern;

	/// <inheritdoc/>
	public override TechniqueGroup TechniqueGroup => TechniqueGroup.SingleDigitPattern;

	/// <inheritdoc/>
	public sealed override Stableness Stableness => base.Stableness;
}
