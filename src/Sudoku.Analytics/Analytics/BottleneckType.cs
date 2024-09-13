namespace Sudoku.Analytics;

/// <summary>
/// Represents a type of bottleneck.
/// </summary>
public enum BottleneckType
{
	/// <summary>
	/// Indicates the bottleneck is the last step in a list of steps that only produces eliminations.
	/// </summary>
	/// <remarks><b>This field can only be used by checking a puzzle containing full-marking steps.</b></remarks>
	EliminationGroup = 0,

	/// <summary>
	/// Indicates the bottleneck is for a hardest step or a list of steps having a same hardest difficulty found in analyzer.
	/// </summary>
	HardestRating,

	/// <summary>
	/// Indicates the bottleneck is for a step or a list of steps that hold the hardest difficulty level.
	/// </summary>
	HardestLevel,

	/// <summary>
	/// <para>Indicates the bottleneck for a step that makes the next step has a easier difficulty level than it.</para>
	/// <para>
	/// For example, the first step is a chain (at "Fiendish" level),
	/// and the second step is a locked candidates (at "Moderate" level).
	/// In this case, the first step makes the second step to be easy, so it will be considered as a bottleneck under this rule.
	/// </para>
	/// </summary>
	SequentialInversion,

	/// <summary>
	/// <para>
	/// Indicates the bottleneck step is a step
	/// whose corresponding grid pattern only contains one step can makes such conclusion with it.
	/// </para>
	/// <para>
	/// For example, the step is a hidden single. However, hidden single is the only step that the current grid can be found.
	/// If you don't apply it to puzzle, solving this puzzle cannot continue.
	/// Therefore, the step will be considered as a bottleneck under this rule.
	/// </para>
	/// </summary>
	/// <remarks><b>This field can only be used by checking a puzzle only containing direct or partial-marking steps.</b></remarks>
	SingleStepOnly,

	/// <summary>
	/// <para>
	/// Indicates the bottleneck step is a step
	/// whose corresponding grid pattern only contains one step can makes such conclusion with it;
	/// and only checks for same-level techniques.
	/// </para>
	/// <para>
	/// For example, the step is a hidden single. However, hidden single is the only step that the current grid can be found.
	/// If you don't apply it to puzzle, solving this puzzle cannot continue.
	/// Therefore, the step will be considered as a bottleneck under this rule;
	/// if the current grid pattern containing complex singles that can have extra assignemnts,
	/// but it will be ignored to be checked in this flag.
	/// </para>
	/// </summary>
	/// <remarks><b>This field can only be used by checking a puzzle only containing direct and partial-marking steps.</b></remarks>
	SingleStepSameLevelOnly,
}
