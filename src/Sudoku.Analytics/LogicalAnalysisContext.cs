namespace Sudoku.Analytics;

/// <summary>
/// Defines a context that is used by step searchers to check the details of the solving and analysis information.
/// </summary>
public ref partial struct LogicalAnalysisContext
{
	/// <summary>
	/// Indicates the puzzle to be solved and analyzed.
	/// </summary>
	/// <remarks>
	/// <include file="../../global-doc-comments.xml" path="/g/csharp11/feature[@name='ref-fields']/target[@name='field']" />
	/// </remarks>
	public readonly ref readonly Grid Grid;


	/// <summary>
	/// Initializes a <see cref="LogicalAnalysisContext"/> instance via the specified.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The reference to the puzzle.</param>
	/// <param name="onlyFindOne">Indicates whether the step searcher only find one possible step and exit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal LogicalAnalysisContext(List<Step>? accumulator, in Grid grid, bool onlyFindOne)
	{
		(Accumulator, OnlyFindOne) = (accumulator, onlyFindOne);
		Grid = ref grid;
	}


	/// <summary>
	/// Indicates whether the solver only find one possible step and exit the searcher.
	/// </summary>
	[MemberNotNullWhen(false, nameof(Accumulator))]
	public bool OnlyFindOne { get; }

	/// <summary>
	/// Indicates the previously set digit.
	/// </summary>
	public int PreviousSetDigit { get; internal set; }

	/// <summary>
	/// <para>
	/// <para>The accumulator to store each step.</para>
	/// </para>
	/// <para>
	/// If <see cref="OnlyFindOne"/> is set to <see langword="true"/>,
	/// this argument will become useless because we only finding one step is okay,
	/// so we may not use the accumulator to store all possible steps, in order to optimize the performance.
	/// Therefore, this argument can be <see langword="null"/> in this case.
	/// </para>
	/// </summary>
	/// <seealso cref="OnlyFindOne"/>
	public List<Step>? Accumulator { get; }
}
