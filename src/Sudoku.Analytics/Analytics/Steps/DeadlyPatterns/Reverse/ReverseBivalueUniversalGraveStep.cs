namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Reverse Bi-value Universal Grave</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="digit1">Indicates the first digit used.</param>
/// <param name="digit2">Indicates the second digit used.</param>
/// <param name="pattern">Indicates the pattern, all possible cells included.</param>
/// <param name="emptyCells">Indicates the empty cells used. This cells have already included in <paramref name="pattern"/>.</param>
public abstract partial class ReverseBivalueUniversalGraveStep(
	Conclusion[] conclusions,
	View[]? views,
	[PrimaryConstructorParameter] Digit digit1,
	[PrimaryConstructorParameter] Digit digit2,
	[PrimaryConstructorParameter(GeneratedMemberName = "CompletePattern")] scoped in CellMap pattern,
	[PrimaryConstructorParameter] scoped in CellMap emptyCells
) : DeadlyPatternStep(conclusions, views), IEquatableStep<ReverseBivalueUniversalGraveStep>
{
	/// <summary>
	/// Indicates whether the pattern is a reverse UR.
	/// </summary>
	public bool IsRectangle => CompletePattern.Count == 4;

	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 6.0M;

	/// <summary>
	/// Indicates the type of the technique.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override Technique Code => Technique.ReverseBivalueUniversalGraveType1 + (short)(Type - 1);

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.Length, A002024(CompletePattern.Count) * .1M)];

	/// <summary>
	/// Indicates the last cells used that are not empty.
	/// </summary>
	public CellMap PatternNonEmptyCells => CompletePattern - EmptyCells;

	private protected string Cell1Str => (Digit1 + 1).ToString();

	private protected string Cell2Str => (Digit2 + 1).ToString();

	private protected string PatternStr => CompletePattern.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<ReverseBivalueUniversalGraveStep>.operator ==(ReverseBivalueUniversalGraveStep left, ReverseBivalueUniversalGraveStep right)
		=> (left.Type, left.CompletePattern, left.Digit1, left.Digit2) == (right.Type, right.CompletePattern, right.Digit1, right.Digit2);
}
