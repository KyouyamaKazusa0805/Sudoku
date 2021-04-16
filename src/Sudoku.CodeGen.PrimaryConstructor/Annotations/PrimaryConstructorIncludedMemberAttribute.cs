using System;

namespace Sudoku.CodeGen.PrimaryConstructor.Annotations
{
	/// <summary>
	/// Indicates the member should be included while generating primary constructors.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class PrimaryConstructorIncludedMemberAttribute : Attribute
	{
	}
}
