using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Represents the type is only used for parameterized object.
	/// You may regard the type as anonymous inner classes (or structs).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class ParameterizedObjectAttribute : Attribute
	{
	}
}
