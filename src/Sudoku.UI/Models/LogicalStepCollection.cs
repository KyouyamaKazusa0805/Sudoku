namespace Sudoku.UI.Models;

/// <summary>
/// Provides with a list of logical steps.
/// </summary>
public sealed class LogicalStepCollection : IEnumerable<LogicalStep>, IReadOnlyList<LogicalStep>
{
	/// <summary>
	/// Indicates the steps.
	/// </summary>
	private readonly IList<LogicalStep> _steps = new List<LogicalStep>();


	/// <summary>
	/// Initializes a <see cref="LogicalStepCollection"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LogicalStepCollection()
	{
	}

	/// <summary>
	/// Initializes a <see cref="LogicalStepCollection"/> instance via the specified steps.
	/// </summary>
	/// <param name="steps">The steps.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LogicalStepCollection(IEnumerable<LogicalStep> steps) => _steps.AddRange(steps);


	/// <inheritdoc cref="ICollection{T}.Count"/>
	public int Count => _steps.Count;


	/// <inheritdoc cref="IList{T}.this[int]"/>
	public LogicalStep this[int index] => _steps[index];


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerator<LogicalStep> GetEnumerator() => _steps.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


	/// <summary>
	/// Creates a <see cref="LogicalStepCollection"/> instance via the specified <see cref="LogicalSolverResult"/>.
	/// </summary>
	/// <param name="steps">The steps.</param>
	/// <returns>A <see cref="LogicalStepCollection"/> instance.</returns>
	/// <seealso cref="LogicalSolverResult"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static LogicalStepCollection Create(LogicalSolverResult steps)
	{
		var list = new List<LogicalStep>();
		foreach (var (grid, step) in steps.SolvingPath)
		{
			list.Add(new() { Step = step, Grid = grid });
		}

		return new(list);
	}
}
