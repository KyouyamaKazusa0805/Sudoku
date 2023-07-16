namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="code"><inheritdoc cref="Step.Code" path="/summary"/></param>
/// <param name="digit1">Indicates the first digit used.</param>
/// <param name="digit2">Indicates the second digit used. This value is always greater than <see cref="Digit1"/>.</param>
/// <param name="cells">Indicates the cells used in this pattern.</param>
/// <param name="isAvoidable">
/// Indicates whether the current rectangle is an avoidable rectangle.
/// If <see langword="true"/>, an avoidable rectangle; otherwise, a unique rectangle.
/// </param>
/// <param name="absoluteOffset">
/// <para>Indicates the absolute offset.</para>
/// <para>
/// The value is an <see cref="int"/> value, as an index, in order to distinct with all unique rectangle patterns.
/// The greater the value is, the later the corresponding pattern will be processed.
/// The value must be between 0 and 485, because the total number of possible patterns is 486.
/// </para>
/// </param>
public abstract partial class UniqueRectangleStep(
	Conclusion[] conclusions,
	View[]? views,
	Technique code,
	[PrimaryConstructorParameter] Digit digit1,
	[PrimaryConstructorParameter] Digit digit2,
	[PrimaryConstructorParameter] scoped in CellMap cells,
	[PrimaryConstructorParameter] bool isAvoidable,
	[PrimaryConstructorParameter] int absoluteOffset
) : DeadlyPatternStep(conclusions, views), IEquatableStep<UniqueRectangleStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <inheritdoc/>
	public sealed override Technique Code => code;

	private protected string D1Str => (Digit1 + 1).ToString();

	private protected string D2Str => (Digit2 + 1).ToString();

	private protected string CellsStr => Cells.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<UniqueRectangleStep>.operator ==(UniqueRectangleStep left, UniqueRectangleStep right)
		=> (left.Code, left.AbsoluteOffset, left.Digit1, left.Digit2) == (right.Code, right.AbsoluteOffset, right.Digit1, right.Digit2)
		&& CandidateMap.Empty + (from conclusion in left.Conclusions select conclusion.Candidate) is var leftConclusion
		&& CandidateMap.Empty + (from conclusion in right.Conclusions select conclusion.Candidate) is var rightConclusion
		&& leftConclusion == rightConclusion;
}
