namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Guardian</b> technique.
/// </summary>
public sealed class GuardianStep(Conclusion[] conclusions, View[]? views, int digit, scoped in CellMap loopCells, scoped in CellMap guardians) :
	NegativeRankStep(conclusions, views),
	IEquatableStep<GuardianStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.5M;

	/// <summary>
	/// Indicates the digit used.
	/// </summary>
	public int Digit { get; } = digit;

	/// <inheritdoc/>
	public override Technique Code => Technique.BrokenWing;

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => DifficultyLevel.Fiendish;

	/// <summary>
	/// Indicates the cells of the loop used.
	/// </summary>
	public CellMap LoopCells { get; } = loopCells;

	/// <summary>
	/// Indicates the guardian cells.
	/// </summary>
	public CellMap Guardians { get; } = guardians;

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> new[] { (ExtraDifficultyCaseNames.Size, A004526(LoopCells.Count + A004526(Guardians.Count)) * .1M) };

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { CellsStr, GuardianSingularOrPlural, GuardianStr } },
			{ "zh", new[] { CellsStr, GuardianSingularOrPlural, GuardianStr } }
		};

	private string CellsStr => LoopCells.ToString();

	private string GuardianSingularOrPlural => R[Guardians.Count == 1 ? "GuardianSingular" : "GuardianPlural"]!;

	private string GuardianStr => Guardians.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(GuardianStep left, GuardianStep right)
		=> (left.Digit, left.LoopCells, left.Guardians) == (right.Digit, right.LoopCells, right.Guardians);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(GuardianStep left, GuardianStep right) => !(left == right);
}
