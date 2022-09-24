namespace Sudoku.Runtime.AnalysisServices;

/// <summary>
/// Defines a context that is used by step searchers to check the details of the solving and analysis information.
/// </summary>
public readonly ref struct LogicalAnalysisContext
{
	/// <summary>
	/// Indicates the puzzle to be solved and analyzed.
	/// </summary>
	/// <remarks>
	/// This field is not encapsulated into a property because C# doesn't support auto read-only properties
	/// returning <see langword="ref"/> or <see langword="ref readonly"/>.
	/// </remarks>
	public readonly ref readonly Grid Grid;


	/// <summary>
	/// Initializes a <see cref="LogicalAnalysisContext"/> instance via the specified.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The reference to the puzzle.</param>
	/// <param name="onlyFindOne">Indicates whether the step searcher only find one possible step and exit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public LogicalAnalysisContext(ICollection<IStep>? accumulator, in Grid grid, bool onlyFindOne)
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
	public ICollection<IStep>? Accumulator { get; }
}
