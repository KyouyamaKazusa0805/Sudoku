using System;

namespace Sudoku.Solving
{
	/// <summary>
	/// Represents a mark on a searcher which should allocate
	/// extremely-high temporary memory.
	/// If so, the manual solver will enable garbage collection forcedly
	/// after finished. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class HighAllocationAttribute : Attribute
	{
	}
}
