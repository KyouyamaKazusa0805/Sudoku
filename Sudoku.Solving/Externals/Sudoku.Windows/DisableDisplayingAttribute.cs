using System;

namespace Sudoku.Windows
{
	/// <summary>
	/// To mark on a technique searcher, to disable displaying in priority tab page in settings window.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, Inherited = false)]
	public sealed class DisableDisplayingAttribute : Attribute
	{
	}
}
