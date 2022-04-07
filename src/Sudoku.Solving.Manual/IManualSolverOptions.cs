namespace Sudoku.Solving.Manual;

/// <summary>
/// Defines an instance that stores the options that bound with a <see cref="ManualSolver"/> instance.
/// </summary>
/// <seealso cref="ManualSolver"/>
public interface IManualSolverOptions
{
	/// <summary>
	/// Indicates whether the solver uses Hodoku mode to solve a sudoku.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <b>Hodoku mode</b> is a mode that imitates a sudoku-solving program called <i>Hodoku</i>
	/// to solve a puzzle. In this case, all enabled step searchers will be in ascending order via their's own
	/// priority, i.e. the value of the property <see cref="SearchingOptions.Priority"/>. On the other hand,
	/// this option provides a basic and normal processing behavior of a <see cref="ManualSolver"/> instance.
	/// </para>
	/// <para>
	/// However, in this case the difficulty order (ascending or descending order) of steps
	/// won't be guaranteed. For example, the possible difficulty rating of a step searched via a UR searcher
	/// is between 4.5 and 4.8, and another rating of a step via a chain searcher is between 4.6 and 5.1.
	/// If the UR searcher has a larger priority than chain searcher, the solver may find all UR steps firsly
	/// in this case, and secondly searches for chains. In some cases, some UR steps found
	/// has a larger difficulty rating value than some chain steps found, so the difficulty ratings
	/// are unstrictly-handled (i.e. steps are out of ordered).
	/// </para>
	/// <para>
	/// If you want to strictly handle the difficulty rating, we recommend you set this property value
	/// as <see langword="false"/>, but the solver will be processed slower than the case setting the
	/// <see langword="true"/> value.
	/// </para>
	/// </remarks>
	/// <seealso cref="SearchingOptions.Priority"/>
	bool IsHodokuMode { get; set; }

	/// <summary>
	/// Indicates whether the solver will use fast searching mode to solve puzzles.
	/// If the value is <see langword="true"/>, the solver will apply all steps after gathered.
	/// In the normal case, solver only fetches the first found step to apply.
	/// </summary>
	bool IsFastSearching { get; set; }

	/// <summary>
	/// Indicates whether the solver will apply the steps via the asecending order.
	/// </summary>
	bool OptimizedApplyingOrder { get; set; }
}
