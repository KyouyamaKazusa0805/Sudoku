namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-Branch W-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="leaves">The leaves of the pattern.</param>
/// <param name="root">The root cells that corresponds to each leaf.</param>
/// <param name="house">Indicates the house that all cells in <see cref="Root"/> lie in.</param>
/// <param name="digitsMask">Indicates the digits used.</param>
public sealed partial class MultiBranchWWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	[PrimaryConstructorParameter] ref readonly CellMap leaves,
	[PrimaryConstructorParameter] ref readonly CellMap root,
	[PrimaryConstructorParameter] House house,
	[PrimaryConstructorParameter] Mask digitsMask
) : IrregularWingStep(conclusions, views, options), ISizeTrait
{
	/// <inheritdoc/>
	public override bool IsGrouped => false;

	/// <inheritdoc/>
	public override bool IsSymmetricPattern => false;

	/// <inheritdoc/>
	public int Size => Leaves.Count;

	/// <inheritdoc/>
	public override int BaseDifficulty => 44;

	/// <inheritdoc/>
	public override Technique Code => Technique.MultiBranchWWing;

	/// <inheritdoc/>
	public override Mask DigitsUsed => DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [LeavesStr, RootStr, HouseStr]), new(SR.ChineseLanguage, [RootStr, HouseStr, LeavesStr])];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_MultiBranchWWingBranchesCountFactor",
				[nameof(Size)],
				GetType(),
				static args => (int)args![0]! == 3 ? 3 : 0
			)
		];

	private string LeavesStr => Options.Converter.CellConverter(Leaves);

	private string RootStr => Options.Converter.CellConverter(Root);

	private string HouseStr => Options.Converter.HouseConverter(1 << House);
}
