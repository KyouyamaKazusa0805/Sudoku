namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Marks on an enumeration type, which means the enumeration type is a closed enumeration type.
	/// </summary>
	/// <remarks>
	/// A <b>closed</b> enumeration type is an enumeration type
	/// of which you can only uses those constant values from declared fields. Therefore <b>all</b>
	/// operators can't be used here.
	/// For example:
	/// <code>
	/// using System.Diagnostics.CodeAnalysis;
	/// 
	/// [Closed]
	/// public enum Gender { Boy; Girl; }
	/// </code>
	/// The code snippet means the type <c>Gender</c> is a closed enumeration type,
	/// so you can't use any arithmetic operators (such as the <see langword="operator"/> <c>+</c>)
	/// to calculate and get the new result of this type, because the operator may get the result that isn't
	/// <c>Gender.Boy</c> or <c>Gender.Girl</c>.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Enum)]
	public sealed class ClosedAttribute : Attribute
	{
	}
}
