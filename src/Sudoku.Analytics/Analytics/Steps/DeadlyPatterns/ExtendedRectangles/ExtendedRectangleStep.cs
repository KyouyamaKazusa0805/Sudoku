using System.Algorithm;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text.Notation;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Extended Rectangle</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells">Indicates the cells used in this pattern.</param>
/// <param name="digitsMask">Indicates the mask of digits used.</param>
public abstract partial class ExtendedRectangleStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] scoped ref readonly CellMap cells,
	[DataMember] Mask digitsMask
) : DeadlyPatternStep(conclusions, views)
{
	/// <inheritdoc/>
	public sealed override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the type of the step. The value must be between 1 and 4.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"ExtendedRectangleType{Type}");

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases => [new(ExtraDifficultyCaseNames.Size, (Sequences.A004526(Cells.Count) - 2) * .1M)];

	private protected string DigitsStr => DigitNotation.ToString(DigitsMask);

	private protected string CellsStr => Cells.ToString();
}
