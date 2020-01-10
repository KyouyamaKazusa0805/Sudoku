using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class OnAddingAttribute : SyntaxContractAttribute
	{
	}
}
