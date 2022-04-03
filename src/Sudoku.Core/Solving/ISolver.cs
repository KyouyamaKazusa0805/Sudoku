namespace Sudoku.Solving;

/// <summary>
/// Defines a simple solver.
/// </summary>
internal interface ISimpleSolver
{
	/// <summary>
	/// To solve the specified grid.
	/// </summary>
	/// <param name="grid">The grid to be solved.</param>
	/// <param name="result">
	/// <para>The result of the grid.</para>
	/// <para>
	/// Please note that if the return value is not <see langword="true"/>,
	/// the value should be a discard and should not be used, because the argument
	/// keeps a memory-randomized value currently.
	/// </para>
	/// </param>
	/// <returns>
	/// A <see cref="bool"/>? value indicating whether the grid can be solved, i.e. has a unique solution.
	/// Please note that the method will return three possible values:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>The puzzle has a unique solution.</description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>The puzzle has multiple solutions.</description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The puzzle has no solution.</description>
	/// </item>
	/// </list>
	/// </returns>
	bool? Solve(in Grid grid, out Grid result);
}
