using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// <para>
	/// Marks members which are only used for completing syntax-based
	/// contract.
	/// </para>
	/// <para>
	/// For example, C# will allow users to use <b>Collection Initialization</b>
	/// syntax to initialize any collections if the target collection object
	/// is confirmed that it has <c>Add</c> method to add new elements.
	/// <see cref="SyntaxContractAttribute"/> suggests you that you should add
	/// <see cref="OnAddingAttribute"/> on <c>Add</c> method.
	/// </para>
	/// </summary>
	public abstract class SyntaxContractAttribute : Attribute
	{
	}
}
