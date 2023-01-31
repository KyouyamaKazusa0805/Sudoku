namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for an auto-notifying field, to define the customized callback method name.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class NotifyCallbackAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="NotifyCallbackAttribute"/> instance via the specified path whose corresponding member will be referenced.
	/// </summary>
	/// <param name="callbackMemberReferencePath">The referenced member path.</param>
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	public NotifyCallbackAttribute(string callbackMemberReferencePath)
	{
	}
}
