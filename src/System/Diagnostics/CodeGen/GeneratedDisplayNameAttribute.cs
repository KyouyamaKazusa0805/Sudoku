namespace System.Diagnostics.CodeGen;

/// <summary>
/// Indicates the display name on generated code of <c>ToString</c> method.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false)]
public sealed class GeneratedDisplayNameAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="GeneratedDisplayNameAttribute"/> instance via the specified name to be displayed.
	/// </summary>
	/// <param name="displayMemberName">The display member name.</param>
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	public GeneratedDisplayNameAttribute(string displayMemberName)
	{
	}
}
