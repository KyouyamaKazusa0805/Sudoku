namespace System.Diagnostics.CodeAnalysis
{
	/// <summary>
	/// <para>
	/// Marks on a field that is a function pointer, which means the function pointer can't be read
	/// or write. In other words, the only reason why the field exposes to the users is that the signature
	/// of the field is useful and helpful.
	/// </para>
	/// <para>
	/// Please note that the field being marked this attribute
	/// should be <see langword="public static readonly"/>.
	/// </para>
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class FunctionPointerExposeOnlyAttribute : Attribute
	{
	}
}
