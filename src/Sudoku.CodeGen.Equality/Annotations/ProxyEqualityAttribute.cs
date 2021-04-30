using System;

namespace Sudoku.CodeGen
{
	/// <summary>
	/// Indicates an attribute instance which is marked on a method,
	/// to tell the users and the compiler that this method is an equality
	/// method to judge whether two instances contain the same value.
	/// </summary>
	/// <remarks>
	/// Because of the limitation of the algorithm and the source generator, the method marked this
	/// attribute must be <see langword="static"/>. If you marks on a method
	/// that isn't a <see langword="static"/> method, the source generator will do nothing.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class ProxyEqualityAttribute : Attribute
	{
	}
}
