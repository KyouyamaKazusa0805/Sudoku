using Sudoku.Collections;

namespace Sudoku.Solving.Manual.Checkers;

/// <summary>
/// Defines a puzzle checker.
/// </summary>
public static class PuzzleChecker
{
	/// <summary>
	/// Indicates the inner solver.
	/// </summary>
	private static readonly BitwiseSolver Solver = new();


	/// <summary>
	/// Checks whether the specified grid contains a valid unique solution.
	/// </summary>
	/// <param name="this">The grid to be checked.</param>
	/// <returns>The <see cref="bool"/> indicating that.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool IsValid(this in Grid @this) =>
		Solver.CheckValidity(@this.ToString(null)) || Solver.CheckValidity(@this.ToString("~"));

	/// <summary>
	/// To check if a puzzle has only one solution or not.
	/// </summary>
	/// <param name="this">The puzzle to check.</param>
	/// <param name="solutionIfValid">
	/// The solution if the puzzle is valid; otherwise, <see cref="Grid.Undefined"/>.
	/// </param>
	/// <param name="sukaku">
	/// Indicates whether the current mode is sukaku mode.
	/// <list type="table">
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
	public static bool IsValid(this in Grid @this, out Grid solutionIfValid, [NotNullWhen(true)] out bool? sukaku)
	{
		if (Solver.CheckValidity(@this.ToString(null), out string? solution))
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
			solutionIfValid = Grid.Undefined;
			sukaku = null;
			return false;
		}
	}
}
