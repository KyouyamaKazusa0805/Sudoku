namespace Sudoku.Analytics;

/// <summary>
/// Defines a context that is used by step searchers to check the details of the solving and analysis information.
/// </summary>
/// <param name="accumulator">The accumulator.</param>
/// <param name="grid">The reference to the puzzle.</param>
/// <param name="onlyFindOne">Indicates whether the step searcher only find one possible step and exit.</param>
public ref struct AnalysisContext(List<Step>? accumulator, [UnscopedRef] in Grid grid, bool onlyFindOne)
{
	/// <summary>
	/// Indicates the backing field of property <see cref="Grid"/>.
	/// </summary>
	private readonly ref readonly Grid _grid = ref grid;


	/// <summary>
	/// Indicates whether the solver only find one possible step and exit the searcher.
	/// </summary>
	[MemberNotNullWhen(false, nameof(Accumulator))]
	public readonly bool OnlyFindOne { get; } = onlyFindOne;

	/// <summary>
	/// Indicates the puzzle to be solved and analyzed.
	/// </summary>
	public readonly ref readonly Grid Grid => ref _grid;

	/// <summary>
	/// <para>The accumulator to store each step.</para>
	/// <para>
	/// If <see cref="OnlyFindOne"/> is set to <see langword="true"/>,
	/// this argument will become useless because we only finding one step is okay,
	/// so we may not use the accumulator to store all possible steps, in order to optimize the performance.
	/// Therefore, this argument can be <see langword="null"/> in this case.
	/// </para>
	/// </summary>
	/// <seealso cref="OnlyFindOne"/>
	public readonly List<Step>? Accumulator { get; } = accumulator;

	/// <summary>
	/// Indicates the previously set digit.
	/// </summary>
	public int PreviousSetDigit { get; internal set; }
}
