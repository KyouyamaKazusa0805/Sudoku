using Sudoku.Collections;
using Sudoku.Solving.Manual.Buffer;
using Sudoku.Solving.Manual.Searchers;
using static Sudoku.Solving.Manual.Buffer.FastProperties;

namespace Sudoku.Solving.Manual;

/// <summary>
/// To mark onto a step searcher, to tell the runtime and the compiler that the type is a step searcher.
/// </summary>
/// <seealso cref="IStepSearcher"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class StepSearcherAttribute : Attribute
{
	/// <summary>
	/// <para>Indicates whether the option is fixed that can't be modified in UI.</para>
	/// <para><i>The default value is <see langword="false"/>.</i></para>
	/// </summary>
	public bool IsOptionsFixed { get; init; }

	/// <summary>
	/// <para>
	/// Indicates the technique searcher can or can't be used in some scenarios
	/// where they aren't in traversing mode to call
	/// <see cref="IStepSearcher.GetAll(ICollection{Step}, in Grid, bool)"/>
	/// in <see cref="IStepSearcher"/>s one by one.
	/// </para>
	/// <para>
	/// If <see langword="true"/>, the searcher can't use those <see langword="static"/>
	/// properties such as <see cref="CandMaps"/> in its method
	/// <see cref="IStepSearcher.GetAll(ICollection{Step}, in Grid, bool)"/>.
	/// </para>
	/// <para><i>The default value is <see langword="false"/>.</i></para>
	/// </summary>
	/// <remarks>
	/// <para>
	/// All disallowed properties are:
	/// <list type="bullet">
	/// <item><see cref="DigitMaps"/></item>
	/// <item><see cref="ValueMaps"/></item>
	/// <item><see cref="CandMaps"/></item>
	/// <item><see cref="BivalueMap"/></item>
	/// <item><see cref="EmptyMap"/></item>
	/// </list>
	/// The disallowed method is:
	/// <list type="bullet">
	/// <item><see cref="InitializeMaps"/></item>
	/// </list>
	/// </para>
	/// <para>
	/// Those properties or methods can optimize the performance to analyze a sudoku grid, but
	/// sometimes they may cause a potential bug that is hard to find and fix. The attribute
	/// is created and used for solving the problem.
	/// </para>
	/// </remarks>
	/// <seealso cref="IStepSearcher"/>
	/// <seealso cref="FastProperties"/>
	public bool IsDirect { get; init; }

	/// <summary>
	/// Indicates whether the specified step searcher doesn't rely on the puzzle.
	/// The default value is <see langword="false"/>.
	/// </summary>
	public bool PuzzleNotRelying { get; init; }
}
