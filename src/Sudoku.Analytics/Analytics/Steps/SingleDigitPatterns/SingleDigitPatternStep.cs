namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Single Digit Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit">Indicates the digit used in this pattern.</param>
public abstract partial class SingleDigitPatternStep(Conclusion[] conclusions, View[]? views, [PrimaryConstructorParameter] Digit digit) :
	Step(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override string Name => base.Name;

	/// <inheritdoc/>
	public sealed override string? Format => base.Format;
}
