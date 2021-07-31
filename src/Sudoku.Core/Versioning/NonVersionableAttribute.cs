using System;

namespace Sudoku.Versioning
{
	/// <summary>
	/// To mark on a member, to tell the user the member is never changed.
	/// </summary>
	/// <remarks>
	/// <para>
	/// In addition, this attribute can also help the author to check the method and update the inner logic.
	/// For example, I implemented a code analyzer that will check this member. If the member
	/// which marks this attribute has been changed its status, the analyzer will be also changed
	/// its inner code logic.
	/// </para>
	/// <para>
	/// This attribute has a different meaning with a type named
	/// <c>System.Runtime.Versioning.NonVersionableAttribute</c>, which doesn't contain
	/// in .NET (.NET 5 and its later version), but in .NET framework. Please tell with them.
	/// </para>
	/// </remarks>
	[AttributeUsage(AttributeTargets.All)]
	public sealed class NonVersionableAttribute : Attribute
	{
	}
}
