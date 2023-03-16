namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Single Digit Pattern</b> technique.
/// </summary>
public abstract class SingleDigitPatternStep(Conclusion[] conclusions, View[]? views, int digit) : Step(conclusions, views)
{
	/// <summary>
	/// Indicates the digit used in this pattern.
	/// </summary>
	public int Digit { get; } = digit;

	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	[AllowNull]
	[MaybeNull]
	public sealed override string Format => base.Format;

	/// <inheritdoc/>
	public override TechniqueGroup Group => TechniqueGroup.SingleDigitPattern;
}
