namespace Sudoku.Solving.Manual;

/// <summary>
/// Indicates an error that throws when a solving step is wrong (may be due to wrong algorithm, bug, etc.).
/// </summary>
[Serializable]
public sealed class WrongStepException : Exception
{
	/// <summary>
	/// Initializes a <see cref="WrongStepException"/> with the specified invalid grid and the wrong step.
	/// </summary>
	/// <param name="grid">The invalid sudoku grid.</param>
	/// <param name="wrongStep">The wrong step.</param>
	public WrongStepException(in Grid grid, Step wrongStep)
	{
		InvalidPuzzle = grid;
		WrongStep = wrongStep;
		Data.Add(nameof(InvalidPuzzle), grid);
		Data.Add(nameof(WrongStep), wrongStep);
	}

	/// <inheritdoc/>
	private WrongStepException(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}


	/// <inheritdoc/>
	public override string Message =>
		$"This current step {WrongStep} may exist bug that causes the wrong handling. " +
		$"The current grid is {InvalidPuzzle:#}";

	/// <inheritdoc/>
	public override string HelpLink =>
		"https://sunnieshine.github.io/Sudoku/types/exceptions/Exception-WrongStepException";

	/// <summary>
	/// Indicates the invalid sudoku grid. This property is also stored in the property
	/// <see cref="Exception.Data"/>.
	/// </summary>
	/// <seealso cref="Exception.Data"/>
	public Grid InvalidPuzzle { get; }

	/// <summary>
	/// Indicates the wrong step.
	/// </summary>
	[DisallowNull]
	public Step? WrongStep { get; }


	/// <inheritdoc/>
	public override void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue(nameof(InvalidPuzzle), $"{InvalidPuzzle:#}", typeof(string));
		info.AddValue(nameof(WrongStep), WrongStep?.ToString() ?? "<unknown step>", typeof(string));

		base.GetObjectData(info, context);
	}
}
