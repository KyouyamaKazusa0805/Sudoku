using System;

namespace Sudoku.CodeGen.DelegatedEquality.Annotations
{
	/// <summary>
	/// Indicates an attribute instance which is marked on a method,
	/// to tell the users and the compiler that this method is an equality
	/// method to judge whether two instances contain the same value.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class DelegatedEqualityMethodAttribute : Attribute
	{
	}
}
