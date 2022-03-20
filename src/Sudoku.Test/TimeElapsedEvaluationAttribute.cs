namespace Sudoku.Test;

/// <summary>
/// Defines an attribute that applies to a field of an enumeration type,
/// indicating the evaluated time elapsed.
/// </summary>
/// <typeparam name="T">The type of the enumeration.</typeparam>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class TimeElapsedEvaluationAttribute<T> : Attribute where T : Enum
{
	/// <summary>
	/// Initializes a <see cref="TimeElapsedEvaluationAttribute{T}"/> instance
	/// via the specified time elapsed evaluated.
	/// </summary>
	/// <param name="timeElapsedEvaluation">The evaluated time elapsed. In milliseconds.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TimeElapsedEvaluationAttribute(int timeElapsedEvaluation) =>
		TimeElapsedEvaluation = timeElapsedEvaluation;


	/// <summary>
	/// Indicates the time elapsed as the evaluated value.
	/// </summary>
	public int TimeElapsedEvaluation { get; }
}
