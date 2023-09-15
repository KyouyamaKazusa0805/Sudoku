using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Concepts;
using Sudoku.Rendering;
using Sudoku.Text.Notation;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Borescoper's Deadly Pattern</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="cells">The map that contains the cells used for this technique.</param>
/// <param name="digitsMask">Indicates the mask of used digits.</param>
public abstract partial class BorescoperDeadlyPatternStep(
	Conclusion[] conclusions,
	View[]? views,
	[DataMember] scoped ref readonly CellMap cells,
	[DataMember] Mask digitsMask
) : DeadlyPatternStep(conclusions, views)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 5.3M;

	/// <summary>
	/// Indicates the type of the technique.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override string Format => GetString($"TechniqueFormat_UniquePolygonType{Type}Step")!;

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"BorescoperDeadlyPatternType{Type}");

	private protected string DigitsStr => DigitNotation.ToString(DigitsMask);

	private protected string CellsStr => Cells.ToString();
}
