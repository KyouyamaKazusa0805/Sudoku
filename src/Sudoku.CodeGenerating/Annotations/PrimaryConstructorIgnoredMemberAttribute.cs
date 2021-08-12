namespace Sudoku.CodeGenerating
{
	/// <summary>
	/// Indicates the member should be ignored while generating primary constructors.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public sealed class PrimaryConstructorIgnoredMemberAttribute : Attribute
	{
	}
}
