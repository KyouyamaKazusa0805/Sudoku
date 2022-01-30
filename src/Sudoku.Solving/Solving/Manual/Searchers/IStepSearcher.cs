using Sudoku.Collections;

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
	/// Accumulate all possible steps into the argument <paramref name="accumulator"/> if
	/// the argument <paramref name="onlyFindOne"/> is <see langword="false"/>,
	/// or return the first found step if the argument <paramref name="onlyFindOne"/>
	/// is <see langword="true"/>.
	/// </summary>
	/// <param name="accumulator">
	/// <para>
	/// <para>The accumulator to store each step.</para>
	/// </para>
	/// <para>
	/// If <paramref name="onlyFindOne"/> is set to <see langword="true"/>,
	/// this argument will become useless because we only finding one step is okay,
	/// so we may not use the accumulator to store all possible steps, in order to optimize the performance.
	/// Therefore, this argument can be <see langword="null"/>
	/// (i.e. the expression <c><see langword="null"/>!</c>) when the argument
	/// <paramref name="onlyFindOne"/> is <see langword="true"/>.
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
