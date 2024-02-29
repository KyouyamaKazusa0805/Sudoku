namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Reverse Bi-value Universal Grave</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1">Indicates the first digit used.</param>
/// <param name="digit2">Indicates the second digit used.</param>
/// <param name="pattern">Indicates the pattern, all possible cells included.</param>
/// <param name="emptyCells">Indicates the empty cells used. This cells have already included in <paramref name="pattern"/>.</param>
public abstract partial class ReverseBivalueUniversalGraveStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Digit digit1,
	[PrimaryConstructorParameter] Digit digit2,
	[PrimaryConstructorParameter(GeneratedMemberName = "CompletePattern")] scoped ref readonly CellMap pattern,
	[PrimaryConstructorParameter] scoped ref readonly CellMap emptyCells
) : DeadlyPatternStep(conclusions, views, options)
{
	/// <inheritdoc/>
	public override bool OnlyUseBivalueCells => false;

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
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [new(ExtraDifficultyFactorNames.Length, A002024(CompletePattern.Count) * .1M)];

	/// <summary>
	/// Indicates the last cells used that are not empty.
	/// </summary>
	public CellMap PatternNonEmptyCells => CompletePattern - EmptyCells;

	private protected string Cell1Str => Options.Converter.DigitConverter((Mask)(1 << Digit1));

	private protected string Cell2Str => Options.Converter.DigitConverter((Mask)(1 << Digit2));

	private protected string PatternStr => Options.Converter.CellConverter(CompletePattern);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is ReverseBivalueUniversalGraveStep comparer
		&& (Type, CompletePattern, Digit1, Digit2) == (comparer.Type, comparer.CompletePattern, comparer.Digit1, comparer.Digit2);
}
