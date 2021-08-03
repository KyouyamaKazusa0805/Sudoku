namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Indicates the marked parameter is a discard and can't be used.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	[Conditional("SOLUTION_WIDE_CODE_ANALYSIS")]
	public sealed class DiscardAttribute : Attribute
	{
	}
}
