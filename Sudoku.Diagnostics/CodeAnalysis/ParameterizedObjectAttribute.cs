using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public sealed class ParameterizedObjectAttribute : Attribute
	{
	}
}
