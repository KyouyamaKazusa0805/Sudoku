namespace Sudoku.UI.Models;

/// <summary>
/// Provides with a list of manual steps.
/// </summary>
public sealed class ManualStepCollection : IEnumerable<ManualStep>, IReadOnlyList<ManualStep>
{
	/// <summary>
	/// Indicates the steps.
	/// </summary>
	private readonly IList<ManualStep> _steps = new List<ManualStep>();


	/// <summary>
	/// Initializes a <see cref="ManualStepCollection"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ManualStepCollection()
	{
	}

	/// <summary>
	/// Initializes a <see cref="ManualStepCollection"/> instance via the specified steps.
	/// </summary>
	/// <param name="steps">The steps.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ManualStepCollection(IEnumerable<ManualStep> steps) => _steps.AddRange(steps);


	/// <inheritdoc cref="ICollection{T}.Count"/>
	public int Count => _steps.Count;


	/// <inheritdoc cref="IList{T}.this[int]"/>
	public ManualStep this[int index] => _steps[index];


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public IEnumerator<ManualStep> GetEnumerator() => _steps.GetEnumerator();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();


	/// <summary>
	/// Creates a <see cref="ManualStepCollection"/> instance via the specified <see cref="LogicalSolverResult"/>.
	/// </summary>
	/// <param name="steps">The steps.</param>
	/// <returns>A <see cref="ManualStepCollection"/> instance.</returns>
	/// <seealso cref="LogicalSolverResult"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static ManualStepCollection Create(LogicalSolverResult steps)
	{
		var list = new List<ManualStep>();
		foreach (var (grid, step) in steps.SolvingPath)
		{
			list.Add(new() { Step = step, Grid = grid });
		}

		return new(list);
	}
}
