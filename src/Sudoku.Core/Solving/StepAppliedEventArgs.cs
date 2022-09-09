namespace Sudoku.Solving;

/// <summary>
/// Provides with data used by event handler type <see cref="StepAppliedEventHandler"/>.
/// </summary>
/// <seealso cref="StepAppliedEventHandler"/>
public sealed class StepAppliedEventArgs : EventArgs
{
	/// <summary>
	/// Determines whether the current data structure stores a single step applied instead of a list of steps.
	/// </summary>
	[MemberNotNullWhen(true, nameof(Step))]
	[MemberNotNullWhen(false, nameof(Steps))]
	public bool IsSingleStepApplied => Step is not null;

	/// <summary>
	/// Indicates the applied step.
	/// </summary>
	public IStep? Step { get; set; }

	/// <summary>
	/// Indicates the applied list of steps.
	/// </summary>
	public IEnumerable<IStep>? Steps { get; set; }
}
