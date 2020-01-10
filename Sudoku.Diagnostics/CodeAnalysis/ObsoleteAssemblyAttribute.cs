using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public sealed class ObsoleteAssemblyAttribute : Attribute
	{
	}
}
