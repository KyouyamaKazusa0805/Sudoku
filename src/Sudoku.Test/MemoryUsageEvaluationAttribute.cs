namespace Sudoku.Test;

/// <summary>
/// Defines an attribute that applies to an enumeratiohn field, which means
/// the current node type will cost the specified size of memory.
/// </summary>
/// <typeparam name="T">The type of the enumeration.</typeparam>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class MemoryUsageEvaluationAttribute<T> : Attribute where T : Enum
{
	/// <summary>
	/// Initializes a <see cref="MemoryUsageEvaluationAttribute{T}"/> instance
	/// via the specified memory allocation evaluation value.
	/// </summary>
	/// <param name="memoryAllocationEvaluation">The allocation value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MemoryUsageEvaluationAttribute(double memoryAllocationEvaluation) =>
		MemoryAllocationEvaluation = memoryAllocationEvaluation;


	/// <summary>
	/// Indicates the evaluated memory usages on this type of node.
	/// </summary>
	public double MemoryAllocationEvaluation { get; }
}
