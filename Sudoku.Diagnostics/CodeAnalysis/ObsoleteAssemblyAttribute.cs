using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// To mark on an assembly, it means that the whole assembly is already obsolete.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	public sealed class ObsoleteAssemblyAttribute : Attribute
	{
	}
}
