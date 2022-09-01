namespace Sudoku.Solving.Manual.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Square Type 4</b> technique.
/// </summary>
/// <param name="Conclusions"><inheritdoc/></param>
/// <param name="Views"><inheritdoc/></param>
/// <param name="Cells"><inheritdoc/></param>
/// <param name="DigitsMask"><inheritdoc/></param>
/// <param name="Digit1">Indicates the digit 1 used.</param>
/// <param name="Digit2">Indicates the digit 2 used.</param>
/// <param name="ConjugateHouse">Indicates the cells used as the conjugate house.</param>
internal sealed partial record UniqueMatrixType4Step(
	ConclusionList Conclusions,
	ViewList Views,
	scoped in CellMap Cells,
	short DigitsMask,
	int Digit1,
	int Digit2,
	scoped in CellMap ConjugateHouse
) : UniqueMatrixStep(Conclusions, Views, Cells, DigitsMask)
{
	/// <inheritdoc/>
	public override int Type => 4;

	[ResourceTextFormatter]
	private partial string ConjStr() => ConjugateHouse.ToString();

	[ResourceTextFormatter]
	private partial string Digit1Str() => (Digit1 + 1).ToString();

	[ResourceTextFormatter]
	private partial string Digit2Str() => (Digit2 + 1).ToString();
}
