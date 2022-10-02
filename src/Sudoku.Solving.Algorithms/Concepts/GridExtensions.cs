namespace Sudoku.Concepts;

/// <summary>
/// Provides with extension methods around solving operations on type <see cref="Grid"/>.
/// </summary>
/// <seealso cref="Grid"/>
public static class GridSolvingExtensions
{
	/// <summary>
	/// Indicates the 
	/// </summary>
	private static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Determines whether the puzzle is valid.
	/// </summary>
	/// <param name="this">The puzzle.</param>
	/// <returns>A <see cref="bool"/> result indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValid(this scoped in Grid @this) => Solver.CheckValidity(@this.ToString());

	/// <summary>
	/// <para>
	/// Determines whether the current grid is valid, checking on both normal and sukaku cases
	/// and returning a <see cref="bool"/>? value indicating whether the current sudoku grid is valid
	/// only on sukaku case.
	/// </para>
	/// <para>
	/// For more information, please see the introduction about the parameter
	/// <paramref name="sukaku"/>.
	/// </para>
	/// </summary>
	/// <param name="this">The puzzle.</param>
	/// <param name="solutionIfValid">
	/// The solution if the puzzle is valid; otherwise, <see cref="Grid.Undefined"/>.
	/// </param>
	/// <param name="sukaku">Indicates whether the current mode is sukaku mode.<list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The puzzle is a sukaku puzzle.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The puzzle is a normal sudoku puzzle.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The puzzle is invalid.</description>
	/// </item>
	/// </list>
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <seealso cref="Grid.Undefined"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool ExactlyValidate(this scoped in Grid @this, out Grid solutionIfValid, [NotNullWhen(true)] out bool? sukaku)
	{
		SkipInit(out solutionIfValid);
		if (Solver.CheckValidity(@this.ToString(null), out var solution))
		{
			solutionIfValid = Grid.Parse(solution);
			sukaku = false;
			return true;
		}
		else if (Solver.CheckValidity(@this.ToString("~"), out solution))
		{
			solutionIfValid = Grid.Parse(solution);
			sukaku = true;
			return true;
		}
		else
		{
			sukaku = null;
			return false;
		}
	}

	/// <summary>
	/// Try to get the solution grid. If failed to solve, <see cref="Grid.Undefined"/> will be returned.
	/// </summary>
	/// <param name="this">The puzzle.</param>
	/// <returns>The solution grid.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid GetSolution(this scoped in Grid @this)
		=> Solver.Solve(@this) switch
		{
			{ IsUndefined: false } solution => solution.UnfixSolution(@this.GivenCells),
			_ => Grid.Undefined
		};
}
