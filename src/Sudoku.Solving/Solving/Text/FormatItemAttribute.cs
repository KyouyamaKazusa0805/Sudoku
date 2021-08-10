using System;
using System.Diagnostics;

namespace Sudoku.Solving.Text
{
	/// <summary>
	/// Marks on a property to tell the user the property is only used for the formatting.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	[Conditional("SOLUTION_WIDE_CODE_ANALYSIS")]
	public sealed class FormatItemAttribute : Attribute
	{
	}
}