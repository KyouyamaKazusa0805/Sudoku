namespace Sudoku.Algorithm.Solving;

/// <summary>
/// Represents a solver that can provide with a basic function to solve a sudoku puzzle given with a <typeparamref name="TGrid"/> instance,
/// and returns its solution grid.
/// </summary>
/// <typeparam name="TGrid"><inheritdoc cref="IGrid{TSelf, TMask, TBitStatusMap, TConclusion}" path="/typeparam[@name='TSelf']"/></typeparam>
/// <typeparam name="TMask"><inheritdoc cref="IGrid{TSelf, TMask, TBitStatusMap, TConclusion}" path="/typeparam[@name='TMask']"/></typeparam>
/// <typeparam name="TBitStatusMap">
/// <inheritdoc cref="IGrid{TSelf, TMask, TBitStatusMap, TConclusion}" path="/typeparam[@name='TBitStatusMap']"/>
/// </typeparam>
/// <typeparam name="TConclusion">
/// <inheritdoc cref="IGrid{TSelf, TMask, TBitStatusMap, TConclusion}" path="/typeparam[@name='TConclusion']"/>
/// </typeparam>
public interface ISolver<TGrid, TMask, TBitStatusMap, TConclusion>
	where TGrid : IGrid<TGrid, TMask, TBitStatusMap, TConclusion>
	where TMask : unmanaged, IBinaryInteger<TMask>
	where TBitStatusMap : unmanaged, IBitStatusMap<TBitStatusMap, Cell>
	where TConclusion : IConclusion<TConclusion, TMask>
{
	/// <summary>
	/// Indicates the URI link that links to the introduction of the algorithm.
	/// </summary>
	/// <remarks>
	/// This property is reserved as information that is offered to the algorithm learners.
	/// </remarks>
	public static abstract string? UriLink { get; }


	/// <summary>
	/// Solve the specified grid, and return the solution via argument <paramref name="result"/>
	/// with returning a <see cref="bool"/>? value indicating the solved state.
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
	public abstract bool? Solve(scoped in TGrid grid, out TGrid result);
}
