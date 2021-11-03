namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher.
/// </summary>
public unsafe interface IStepSearcher
{
	/// <summary>
	/// Indicates the step searching options.
	/// </summary>
	SearchingOptions Options { get; set; }

	/// <summary>
	/// Checks the specified grid to get the result whether the specified grid can use this technique.
	/// </summary>
	/// <returns>
	/// Returns a function pointer that executes and checks a sudoku grid puzzle,
	/// and then returns a <see cref="bool"/> result when:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>The description</description>
	/// </listheader>
	/// <item>
	/// <term><c><see langword="true"/></c></term>
	/// <description>The grid can use this step searcher.</description>
	/// </item>
	/// <item>
	/// <term><c><see langword="false"/></c></term>
	/// <description>
	/// The grid may be useless to use this step searcher because it doesn't satisfy the certain pre-conditions.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// Please note that the property will return a function pointer,
	/// not a method result after being executed.
	/// </remarks>
	delegate*<in Grid, bool> Predicate { get; }


	/// <summary>
	/// Accumulate all possible steps into the specified accumulator.
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
