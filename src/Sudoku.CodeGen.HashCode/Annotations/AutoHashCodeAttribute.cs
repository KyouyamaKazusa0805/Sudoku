using System;

namespace Sudoku.CodeGen.HashCode.Annotations
{
	/// <summary>
	/// Indicates an attribute that marks a <see langword="class"/> or a <see langword="struct"/>
	/// that tells the compiler the type should generate a default <c>GetHashCode</c> method.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class AutoHashCodeAttribute : Attribute
	{
	}
}
