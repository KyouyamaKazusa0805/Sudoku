using System;

namespace Sudoku.Solving.Annotations
{
	/// <summary>
	/// Marks on a technique searcher to describe the searcher is only used in analyzing a sudoku grid.
	/// When in find-all-step mode, this searcher will be disabled.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class OnlyEnableInAnalysisAttribute : Attribute
	{
	}
}
