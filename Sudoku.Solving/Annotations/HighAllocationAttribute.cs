using System;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// Represents a mark on a searcher which should allocate
	/// extremely-high temporary memory.
	/// If so, the manual solver will enable garbage collection forcedly
	/// after finished. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	[Obsolete("Please use the property" + nameof(DisabledReason.HighAllocation) + "instead.", true)]
	public sealed class HighAllocationAttribute : Attribute
	{
	}
}
