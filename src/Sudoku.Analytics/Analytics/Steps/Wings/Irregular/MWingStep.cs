namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>M-Wing</b> technique.
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
	[RecordParameter] scoped ref readonly CellMap node1,
	[RecordParameter] scoped ref readonly CellMap node2,
	[RecordParameter] Cell strongXyCell,
	[RecordParameter] Cell weakXyCell,
	[RecordParameter] Mask digitsMask
) : IrregularWingStep(conclusions, views, options)
{
	/// <summary>
	/// Indicates whether the pattern is grouped.
	/// </summary>
	public bool IsGrouped => Node1.Count != 1 || Node2.Count != 1;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.IsGrouped, .1M)];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [
			new(EnglishLanguage, [Node1Str, Node2Str, Cell1Str, Cell2Str]),
			new(ChineseLanguage, [Node1Str, Node2Str, Cell1Str, Cell2Str])
		];

	/// <inheritdoc/>
	public override Technique Code => IsGrouped ? Technique.GroupedMWing : Technique.MWing;

	private string Node1Str => Options.Converter.CellConverter(Node1);

	private string Node2Str => Options.Converter.CellConverter(Node2);

	private string Cell1Str => Options.Converter.CellConverter([WeakXyCell]);

	private string Cell2Str => Options.Converter.CellConverter([StrongXyCell]);
}
