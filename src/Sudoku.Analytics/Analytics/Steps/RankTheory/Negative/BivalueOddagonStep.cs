using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Bi-value Oddagon</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="loopCells">Indicates the loop of cells used.</param>
/// <param name="digit1">Indicates the first digit used.</param>
/// <param name="digit2">Indicates the second digit used.</param>
public abstract partial class BivalueOddagonStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] scoped ref readonly CellMap loopCells,
	[Data] Digit digit1,
	[Data] Digit digit2
) : NegativeRankStep(conclusions, views, options), IEquatableStep<BivalueOddagonStep>
{
	/// <summary>
	/// Indicates the type of the technique.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public override decimal BaseDifficulty => 6.3M;

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"BivalueOddagonType{Type}");

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.Size, (LoopCells.Count >> 1) * .1M)];

	private protected string LoopStr => Options.Converter.CellConverter(LoopCells);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<BivalueOddagonStep>.operator ==(BivalueOddagonStep left, BivalueOddagonStep right)
		=> (left.Type, left.Digit1, left.Digit2, left.LoopCells) == (right.Type, right.Digit1, right.Digit2, right.LoopCells);
}
