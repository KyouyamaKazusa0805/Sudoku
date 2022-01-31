using Sudoku.Data;
using Sudoku.Presentation;
using Sudoku.Solving.Manual.Text;
using Sudoku.Techniques;

namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Full House</b> technique.
/// </summary>
/// <param name="Cell"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Digit"><inheritdoc/></param>
public sealed record FullHouseStep(
	ImmutableArray<Conclusion> Conclusions,
	ImmutableArray<PresentationData> Views,
	int Cell,
	int Digit
) : SingleStep(Conclusions, Views, Cell, Digit)
{
	/// <inheritdoc/>
	public override decimal Difficulty => 1.0M;

	/// <inheritdoc/>
	public override Technique TechniqueCode => Technique.FullHouse;

	/// <inheritdoc/>
	public override Rarity Rarity => Rarity.Always;

	[FormatItem]
	private string CellStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new Coordinate((byte)Cell).ToString();
	}

	[FormatItem]
	private string DigitStr
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => (Digit + 1).ToString();
	}
}
