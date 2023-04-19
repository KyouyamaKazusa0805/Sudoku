namespace Sudoku.Analytics;

/// <summary>
/// Defines a context that is used by step searchers to check the details of the solving and analysis information.
/// </summary>
/// <param name="accumulator">
/// <para>The accumulator to store each step.</para>
/// <para>
/// If <see cref="OnlyFindOne"/> is set to <see langword="true"/>,
/// this argument will become useless because we only finding one step is okay,
/// so we may not use the accumulator to store all possible steps, in order to optimize the performance.
/// Therefore, this argument can be <see langword="null"/> in this case.
/// </para>
/// </param>
/// <param name="grid">Indicates the puzzle to be solved and analyzed.</param>
/// <param name="onlyFindOne">Indicates whether the solver only find one possible step and exit the searcher.</param>
[StructLayout(LayoutKind.Auto)]
public unsafe ref partial struct AnalysisContext(
	[PrimaryConstructorParameter] List<Step>? accumulator,
	[PrimaryConstructorParameter(MemberKinds.Field)] in Grid grid,
	[PrimaryConstructorParameter(MembersNotNull = "false: Accumulator")] bool onlyFindOne
)
{
	/// <summary>
	/// Indicates the puzzle to be solved and analyzed.
	/// </summary>
	public readonly ref readonly Grid Grid => ref _grid;

	/// <summary>
	/// Indicates the previously set digit.
	/// </summary>
	public int PreviousSetDigit { get; internal set; }
}
