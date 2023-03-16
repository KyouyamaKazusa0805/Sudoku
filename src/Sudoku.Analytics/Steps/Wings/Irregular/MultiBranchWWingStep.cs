namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Multi-Branch W-Wing</b> technique.
/// </summary>
public sealed class MultiBranchWWingStep(Conclusion[] conclusions, View[]? views, scoped in CellMap leaves, scoped in CellMap root, int house) :
	IrregularWingStep(conclusions, views)
{
	/// <summary>
	/// Indicates the number of branches of the technique.
	/// </summary>
	public int Size => Leaves.Count;

	/// <summary>
	/// Indicates the house that all cells in <see cref="Root"/> lie in.
	/// </summary>
	public int House { get; } = house;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.4M;

	/// <inheritdoc/>
	public override Technique Code => Technique.MultiBranchWWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new ExtraDifficultyCase[] { new(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => .3M, 4 => .6M, 5 => 1.0M, _ => 1.4M }) };

	/// <summary>
	/// The leaves of the pattern.
	/// </summary>
	public CellMap Leaves { get; } = leaves;

	/// <summary>
	/// The root cells that corresponds to each leaf.
	/// </summary>
	public CellMap Root { get; } = root;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { LeavesStr, RootStr, HouseStr } },
			{ "zh", new[] { RootStr, HouseStr, LeavesStr } }
		};

	private string LeavesStr => RxCyNotation.ToCellsString(Leaves);

	private string RootStr => RxCyNotation.ToCellsString(Root);

	private string HouseStr => HouseFormatter.Format(House);
}
