namespace Sudoku.Solving.Manual.Searchers;

/// <summary>
/// Defines a step searcher.
/// </summary>
public interface IStepSearcher
{
	/// <summary>
	/// Indicates the value that identifies which type the step searcher is of.
	/// The value may be also used in UI rendering.
	/// </summary>
	SearcherIdentifier Identifier { get; }

	/// <summary>
	/// Indicates the step searching options.
	/// </summary>
	SearchingOptions Options { get; set; }


	/// <summary>
	/// Accumulate all possible steps into the specified accumulator.
	/// </summary>
	/// <param name="accumulator">The accumulator to store each step.</param>
	/// <param name="grid">The grid to search for techniques.</param>
	/// <param name="onlyFindOne">
	/// Indicates whether the method only searches for one <see cref="Step"/> instance.
	/// </param>
	/// <returns>
	/// Returns the first found step. The nullability of the return value are as belows:
	/// <list type="table">
	/// <listheader>
	/// <term>Nullability</term>
	/// <description>Description of this case</description>
	/// </listheader>
	/// <item>
	/// <term>Maybe <see langword="null"/></term>
	/// <description>
	/// The argument <paramref name="onlyFindOne"/> is <see langword="false"/>,
	/// or the argument <paramref name="onlyFindOne"/> is <see langword="true"/>, but nothing found in this method.
	/// </description>
	/// </item>
	/// <item>
	/// <term>Not <see langword="null"/></term>
	/// <description>
	/// The argument <paramref name="onlyFindOne"/> is <see langword="true"/>, and found at least one step.
	/// In this case the return value is the first found step.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	Step? GetAll(ICollection<Step> accumulator, in Grid grid, bool onlyFindOne);
}
