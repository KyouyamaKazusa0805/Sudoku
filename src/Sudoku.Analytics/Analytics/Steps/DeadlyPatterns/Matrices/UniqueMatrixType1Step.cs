namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Matrix Type 1</b> technique.
/// </summary>
public sealed class UniqueMatrixType1Step(Conclusion[] conclusions, View[]? views, scoped in CellMap cells, Mask digitsMask, int candidate) :
	UniqueMatrixStep(conclusions, views, cells, digitsMask)
{
	/// <inheritdoc/>
	public override int Type => 1;

	/// <summary>
	/// Indicates the true candidate.
	/// </summary>
	public int Candidate { get; } = candidate;

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", new[] { DigitsStr, CellsStr, CandidateStr } },
			{ "zh", new[] { CandidateStr, CellsStr, DigitsStr } }
		};

	private string CandidateStr => (CandidateMap.Empty + Candidate).ToString();
}
