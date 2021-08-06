namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// To mark on an assembly, to tell the user and the compiler that the assembly is obsolete.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	[Conditional("SOLUTION_WIDE_CODE_ANALYSIS")]
	public sealed class AssemblyObsoleteAttribute : Attribute
	{
	}
}