using System.Runtime.InteropServices;
using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts;

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
/// <param name="predefinedOptions">
/// Indicates the pre-defined options set by user in type <see cref="Analyzer"/>.
/// The value can be <see langword="null"/> if the target step searcher doesn't use this property.
/// </param>
/// <seealso cref="Analyzer"/>
[StructLayout(LayoutKind.Auto)]
public ref partial struct AnalysisContext(
	[DataMember] List<Step>? accumulator,
	[DataMember(MemberKinds.Field)] in Grid grid,
	[DataMember(MembersNotNull = "false: Accumulator")] bool onlyFindOne,
	[DataMember] StepSearcherOptions predefinedOptions
)
{
	/// <summary>
	/// Indicates the puzzle to be solved and analyzed.
	/// </summary>
	public readonly ref readonly Grid Grid => ref _grid;

	/// <summary>
	/// Indicates the previously set digit.
	/// </summary>
	public Digit PreviousSetDigit { get; internal set; }
}
