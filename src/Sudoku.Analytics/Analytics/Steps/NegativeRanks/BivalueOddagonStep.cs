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
	[PrimaryConstructorParameter] scoped ref readonly CellMap loopCells,
	[PrimaryConstructorParameter] Digit digit1,
	[PrimaryConstructorParameter] Digit digit2
) : NegativeRankStep(conclusions, views, options)
{
	/// <summary>
	/// Indicates the type of the technique.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public override int BaseDifficulty => 63;

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"BivalueOddagonType{Type}");

	/// <inheritdoc/>
	public override FactorCollection Factors => [new BivalueOddagonLengthFactor()];

	private protected string LoopStr => Options.Converter.CellConverter(LoopCells);


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is BivalueOddagonStep comparer
		&& (Type, Digit1, Digit2, LoopCells) == (comparer.Type, comparer.Digit1, comparer.Digit2, comparer.LoopCells);

	/// <inheritdoc/>
	public override int CompareTo(Step? other)
	{
		if (other is not BivalueOddagonStep comparer)
		{
			return 1;
		}

		var r1 = Math.Abs(LoopCells.Count - comparer.LoopCells.Count);
		if (r1 != 0)
		{
			return r1;
		}

		return Math.Abs(Code - comparer.Code);
	}
}
