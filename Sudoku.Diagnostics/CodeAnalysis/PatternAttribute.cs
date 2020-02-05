using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// It means the parameter marked this attribute is a regular expression pattern.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public sealed class PatternAttribute : Attribute
	{
	}
}
