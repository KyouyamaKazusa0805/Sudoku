namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Bivalue Universal Grave Type 1</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
public sealed partial class HiddenBivalueUniversalGraveType1Step(ReadOnlyMemory<Conclusion> conclusions, View[]? views, StepGathererOptions options) :
	HiddenBivalueUniversalGraveStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <inheritdoc/>
	public override Mask DigitsUsed => (Mask)(1 << Conclusions.Span[0].Digit);

	/// <inheritdoc/>
	public override Technique Code => Technique.HiddenBivalueUniversalGraveType1;
}
