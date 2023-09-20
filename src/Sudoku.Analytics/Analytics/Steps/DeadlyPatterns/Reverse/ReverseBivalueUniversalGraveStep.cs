using System.Algorithm;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text.Notation;

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
	[DataMember] Digit digit1,
	[DataMember] Digit digit2,
	[DataMember(GeneratedMemberName = "CompletePattern")] scoped ref readonly CellMap pattern,
	[DataMember] scoped ref readonly CellMap emptyCells
) : DeadlyPatternStep(conclusions, views, options), IEquatableStep<ReverseBivalueUniversalGraveStep>
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
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [new(ExtraDifficultyCaseNames.Length, Sequences.A002024(CompletePattern.Count) * .1M)];

	/// <summary>
	/// Indicates the last cells used that are not empty.
	/// </summary>
	public CellMap PatternNonEmptyCells => CompletePattern - EmptyCells;

	private protected string Cell1Str => DigitNotation.ToString(Digit1);

	private protected string Cell2Str => DigitNotation.ToString(Digit2);

	private protected string PatternStr => CompletePattern.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<ReverseBivalueUniversalGraveStep>.operator ==(ReverseBivalueUniversalGraveStep left, ReverseBivalueUniversalGraveStep right)
		=> (left.Type, left.CompletePattern, left.Digit1, left.Digit2) == (right.Type, right.CompletePattern, right.Digit1, right.Digit2);
}
