using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// To mark on the method, it means that the method is used for add elements
	/// in this current collection.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class OnAddingAttribute : SyntaxContractAttribute
	{
	}
}
