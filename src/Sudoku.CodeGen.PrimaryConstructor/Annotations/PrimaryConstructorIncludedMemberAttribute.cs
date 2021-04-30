using System;

namespace Sudoku.CodeGen
{
	/// <summary>
	/// Indicates the member should be included while generating primary constructors.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class PrimaryConstructorIncludedMemberAttribute : Attribute
	{
	}
}
