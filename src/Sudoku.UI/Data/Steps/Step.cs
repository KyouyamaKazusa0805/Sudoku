namespace Sudoku.UI.Data.Steps;

/// <summary>
/// Defines a step that allows the user undoing and redoing such given operations.
/// </summary>
/// <param name="Grid">Indicates the sudoku grid that binds with the current step.</param>
public abstract record Step(in Grid Grid) :
	IEquatable<Step>
#if FEATURE_GENERIC_MATH
	,
	IEqualityOperators<Step, Step>
#endif
{
	/// <summary>
	/// Indicates the type of the current step.
	/// </summary>
	protected abstract string StepTypeName { get; }

	/// <summary>
	/// Indicates the description of the current step.
	/// </summary>
	protected abstract string StepDescription { get; }
	

	/// <summary>
	/// Gets the string representation of the current step.
	/// </summary>
	/// <returns>The string representation of the current step.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public sealed override string ToString() => $"{StepTypeName}: {StepDescription}";
}
