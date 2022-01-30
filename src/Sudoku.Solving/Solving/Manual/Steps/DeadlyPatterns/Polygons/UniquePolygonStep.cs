using Sudoku.Collections;
using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps.DeadlyPatterns.Polygons;

/// <summary>
/// Provides with a step that is a <b>Unique Polygon</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Map">The map that contains the cells used for this technique.</param>
/// <param name="DigitsMask">The mask that contains all digits used.</param>
public abstract record UniquePolygonStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	in Cells Map,
	short DigitsMask
) : DeadlyPatternStep(Conclusions, Views)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 5.3M;

	/// <inheritdoc/>
	public sealed override DifficultyLevel DifficultyLevel => DifficultyLevel.Hard;

	/// <inheritdoc/>
	public sealed override TechniqueGroup TechniqueGroup => TechniqueGroup.DeadlyPattern;

	/// <inheritdoc/>
	public sealed override Rarity Rarity => Rarity.HardlyEver;

	/// <inheritdoc/>
	public abstract override Technique TechniqueCode { get; }

	/// <summary>
	/// Indicates the digits string.
	/// </summary>
	[FormatItem]
	protected string DigitsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new DigitCollection(DigitsMask).ToString();
	}

	/// <summary>
	/// Indicates the cells string.
	/// </summary>
	[FormatItem]
	protected string CellsStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => Map.ToString();
	}
}
