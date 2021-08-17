namespace Sudoku;

/// <summary>
/// Indicates an error that throws when a sudoku grid has multiple solutions
/// while solving, checking or generating a puzzle.
/// </summary>
[Serializable]
public sealed class MultipleSolutionsException : Exception
{
	/// <summary>
	/// Initializes a <see cref="NoSolutionException"/> with the specified invalid grid.
	/// </summary>
	/// <param name="grid">The invalid sudoku grid.</param>
	public MultipleSolutionsException(in Grid grid)
	{
		InvalidPuzzle = grid;
		Data.Add(nameof(InvalidPuzzle), grid);
	}

	/// <summary>
	/// Initializes a <see cref="MultipleSolutionsException"/> with the specified invalid grid.
	/// </summary>
	/// <param name="grid">The invalid sudoku grid.</param>
	[Obsolete("Please use another constructor instead.", false)]
	public MultipleSolutionsException(in SudokuGrid grid)
	{
		InvalidGrid = grid;
		Data.Add(nameof(InvalidGrid), grid);
	}

	/// <inheritdoc/>
	private MultipleSolutionsException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}


	/// <inheritdoc/>
	public override string Message => InvalidPuzzle.IsUndefined
		? $"This grid {InvalidGrid:#} contains multiple solutions."
		: $"This grid {InvalidPuzzle:#} contains multiple solutions.";

	/// <inheritdoc/>
	public override string HelpLink =>
		"https://sunnieshine.github.io/Sudoku/types/exceptions/Exception-MultipleSolutionsException";


	/// <summary>
	/// Indicates the invalid sudoku grid. This property is also stored in the property
	/// <see cref="Exception.Data"/>.
	/// </summary>
	/// <seealso cref="Exception.Data"/>
	public Grid InvalidPuzzle { get; } = Grid.Undefined;

	/// <summary>
	/// Indicates the invalid sudoku grid. This property is also stored in the property
	/// <see cref="Exception.Data"/>.
	/// </summary>
	/// <seealso cref="Exception.Data"/>
	[Obsolete($"Please use the property '{nameof(InvalidPuzzle)}' instead.", false)]
	public SudokuGrid InvalidGrid { get; } = SudokuGrid.Undefined;


	/// <inheritdoc/>
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		if (InvalidPuzzle.IsUndefined)
		{
			info.AddValue(nameof(InvalidGrid), InvalidGrid.ToString("#"), typeof(string));
		}
		else
		{
			info.AddValue(nameof(InvalidPuzzle), InvalidPuzzle.ToString("#"), typeof(string));
		}

		base.GetObjectData(info, context);
	}
}
