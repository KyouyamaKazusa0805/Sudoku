namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher.
/// </summary>
public interface IStepSearcher
{
	/// <summary>
	/// Indicates the step searching options.
	/// </summary>
	SearchingOptions Options { get; set; }


	/// <summary>
	/// Accumulate all possible steps into the specified accumulator.
	/// </summary>
	/// <param name="accumulator">
	/// <para>The accumulator to store each step.</para>
	/// <para>
	/// If <paramref name="onlyFindOne"/> is set to <see langword="true"/>,
	/// this argument will become useless because we only finding one step is okay,
	/// so we may not use the accumulator to store all possible steps, in order to optimize the performance.
	/// </para>
	/// </param>
	/// <param name="grid">The grid to search for techniques.</param>
	/// <param name="onlyFindOne">
	/// Indicates whether the method only searches for one <see cref="Step"/> instance.
	/// </param>
	/// <returns>
	/// Returns the first found step. The nullability of the return value are as belows:
	/// <list type="bullet">
	/// <item>
	/// <see langword="null"/>:
	/// <list type="bullet">
	/// <item><c><paramref name="onlyFindOne"/> == <see langword="false"/></c>.</item>
	/// <item><c><paramref name="onlyFindOne"/> == <see langword="true"/></c>, but <b>nothing</b> found.</item>
	/// </list>
	/// </item>
	/// <item>
	/// Not <see langword="null"/>:
	/// <list type="bullet">
	/// <item>
	/// <c><paramref name="onlyFindOne"/> == <see langword="true"/></c>, and found <b>at least one step</b>.
	/// In this case the return value is the first found step.
	/// </item>
	/// </list>
	/// </item>
	/// </list>
	/// </returns>
	Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne);
}
