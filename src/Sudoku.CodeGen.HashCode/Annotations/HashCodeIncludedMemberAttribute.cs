using System;

namespace Sudoku.CodeGen.HashCode.Annotations
{
	/// <summary>
	/// Indicates an attribute that marks a field or a property that tells the compiler the member
	/// will participate in the hash code calculation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class HashCodeIncludedMemberAttribute : Attribute
	{
	}
}
