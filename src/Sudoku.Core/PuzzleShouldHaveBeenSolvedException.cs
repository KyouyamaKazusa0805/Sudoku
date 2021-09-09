namespace Sudoku;

/// <summary>
/// Indicates an exception that throws when the puzzle should have been solved before using a function.
/// </summary>
[Serializable]
public sealed class PuzzleShouldHaveBeenSolvedException : Exception
{
	/// <summary>
	/// Initializes a <see cref="PuzzleShouldHaveBeenSolvedException"/> instance with the specified puzzle.
	/// </summary>
	/// <param name="grid">The grid.</param>
	public PuzzleShouldHaveBeenSolvedException(in SudokuGrid grid)
	{
		InvalidGrid = grid;
		Data.Add(nameof(InvalidGrid), grid);
	}

	/// <inheritdoc/>
	private PuzzleShouldHaveBeenSolvedException(SerializationInfo info, StreamingContext context)
	: base(info, context)
	{
	}


	/// <inheritdoc/>
	public override string Message =>
		$"The puzzle {InvalidGrid:#} should have already been solved before using the current function.";

	/// <inheritdoc/>
	public override string HelpLink =>
		"https://sunnieshine.github.io/Sudoku/types/exceptions/Exception-PuzzleShouldHaveBeenSolvedException";

	/// <summary>
	/// Indicates the invalid grid.
	/// </summary>
	public SudokuGrid InvalidGrid { get; }


	/// <inheritdoc/>
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue(nameof(InvalidGrid), InvalidGrid.ToString("#"), typeof(string));

		base.GetObjectData(info, context);
	}
}
