namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Rectangle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
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
	StepSearcherOptions options,
	[PrimaryConstructorParameter(Accessibility = "public sealed override")] Technique code,
	[PrimaryConstructorParameter] Digit digit1,
	[PrimaryConstructorParameter] Digit digit2,
	[PrimaryConstructorParameter] scoped ref readonly CellMap cells,
	[PrimaryConstructorParameter] bool isAvoidable,
	[PrimaryConstructorParameter] int absoluteOffset
) : DeadlyPatternStep(conclusions, views, options), IComparableStep<UniqueRectangleStep>, IEquatableStep<UniqueRectangleStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	private protected string D1Str => Options.Converter.DigitConverter((Mask)(1 << Digit1));

	private protected string D2Str => Options.Converter.DigitConverter((Mask)(1 << Digit2));

	private protected string CellsStr => Options.Converter.CellConverter(Cells);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static int IComparableStep<UniqueRectangleStep>.Compare(UniqueRectangleStep left, UniqueRectangleStep right)
		=> Math.Sign(left.Code - right.Code) switch { 0 => Math.Sign(left.AbsoluteOffset - right.AbsoluteOffset), var result => result };


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<UniqueRectangleStep>.operator ==(UniqueRectangleStep left, UniqueRectangleStep right)
		=> (left.Code, left.AbsoluteOffset, left.Digit1, left.Digit2) == (right.Code, right.AbsoluteOffset, right.Digit1, right.Digit2)
		&& (CandidateMap)([.. from conclusion in left.Conclusions select conclusion.Candidate]) is var l
		&& (CandidateMap)([.. from conclusion in right.Conclusions select conclusion.Candidate]) is var r
		&& l == r;
}
