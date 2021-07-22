namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// Marks on a generic type parameter, to tell the compiler that the type is a <see langword="ref struct"/>.
	/// </summary>
	[AttributeUsage(AttributeTargets.GenericParameter)]
	public sealed class RefStructTypeAttribute : Attribute
	{
	}
}
