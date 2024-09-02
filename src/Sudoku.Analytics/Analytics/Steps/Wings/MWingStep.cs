namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>(Grouped) M-Wing</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="node1">Indicates the node 1.</param>
/// <param name="node2">Indicates the node 2.</param>
/// <param name="strongXyCell">Indicates the strong XY cell.</param>
/// <param name="weakXyCell">Indicates the weak XY cell.</param>
/// <param name="digitsMask">Indicates the digits used.</param>
public sealed partial class MWingStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] ref readonly CellMap node1,
	[PrimaryConstructorParameter] ref readonly CellMap node2,
	[PrimaryConstructorParameter] Cell strongXyCell,
	[PrimaryConstructorParameter] Cell weakXyCell,
	[PrimaryConstructorParameter] Mask digitsMask
) : IrregularWingStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool IsSymmetricPattern => false;

	/// <inheritdoc/>
	public override bool IsGrouped => Node1.Count != 1 || Node2.Count != 1;

	/// <inheritdoc/>
	public override int BaseDifficulty => 45;

	/// <inheritdoc/>
	public override Mask DigitsUsed => DigitsMask;

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, [Node1Str, Node2Str, Cell1Str, Cell2Str]),
			new(SR.ChineseLanguage, [Node1Str, Node2Str, Cell1Str, Cell2Str])
		];

	/// <inheritdoc/>
	public override Technique Code => IsGrouped ? Technique.GroupedMWing : Technique.MWing;

	private string Node1Str => Options.Converter.CellConverter(Node1);

	private string Node2Str => Options.Converter.CellConverter(Node2);

	private string Cell1Str => Options.Converter.CellConverter(in WeakXyCell.AsCellMap());

	private string Cell2Str => Options.Converter.CellConverter(in StrongXyCell.AsCellMap());


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is MWingStep comparer && Node1 == comparer.Node1 && Node2 == comparer.Node2
		&& StrongXyCell == comparer.StrongXyCell && WeakXyCell == comparer.WeakXyCell;
}
