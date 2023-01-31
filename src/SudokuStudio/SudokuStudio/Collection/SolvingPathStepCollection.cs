namespace SudokuStudio.Collection;

/// <summary>
/// Defines a solving path.
/// </summary>
public sealed class SolvingPathStepCollection : List<SolvingPathStep>
{
	/// <summary>
	/// Initializes a <see cref="SolvingPathStepCollection"/> instance.
	/// </summary>
	public SolvingPathStepCollection() : base()
	{
	}

	/// <summary>
	/// Initializes a <see cref="SolvingPathStepCollection"/> instance via the specified collection of solving path steps.
	/// </summary>
	/// <param name="steps">The solving path steps.</param>
	public SolvingPathStepCollection(IEnumerable<SolvingPathStep> steps) : base(steps)
	{
	}


	/// <summary>
	/// Creates a <see cref="SolvingPathStepCollection"/> instance via the specified <see cref="LogicalSolverResult"/> instance.
	/// </summary>
	/// <param name="analysisResult">A <see cref="LogicalSolverResult"/> instance.</param>
	/// <param name="displayKind">Indicates all displaying values.</param>
	/// <returns>An instance of the current type.</returns>
	public static SolvingPathStepCollection Create(LogicalSolverResult analysisResult, StepTooltipDisplayKind displayKind)
	{
		if (analysisResult is not { IsSolved: true, SolvingPath: { IsDefaultOrEmpty: false, Length: var pathStepsCount } steps })
		{
			return new();
		}

		var collection = new List<SolvingPathStep>();
		for (var i = 0; i < pathStepsCount; i++)
		{
			var (sGrid, s) = steps[i];
			collection.Add(new() { Index = i, StepGrid = sGrid, Step = s, DisplayKinds = displayKind });
		}

		return new(collection);
	}
}
