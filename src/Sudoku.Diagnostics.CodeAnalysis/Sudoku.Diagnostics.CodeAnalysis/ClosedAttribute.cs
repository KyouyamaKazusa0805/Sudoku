using System;

namespace Sudoku.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Marks on a enumeration type, which means the enumeration type is a closed enumeration type.
	/// </summary>
	/// <remarks>
	/// A closed enumeration type is a type that only holds the value in the enumeration declaration field
	/// (i.e. the curly bracket). For example, the code
	/// <code>
	/// [Closed] public enum Gender { Boy; Girl; }
	/// </code>
	/// Which means the type <c>Gender</c> is a closed enumeration type, and you can't use arithmetic operators
	/// (such as <c>operator +</c>) to get the fields that don't appear
	/// in the declaration <c>{ Boy; Girl; }</c> (e.g. <c>Gender.Boy + Gender.Girl</c>,
	/// <c>Gender.Boy + 1</c>, etc.). In other words, you can only use those fields as the references
	/// instead of any calculations.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Enum)]
	public sealed class ClosedAttribute : Attribute
	{
	}
}
