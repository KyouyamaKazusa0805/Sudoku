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
	public NotifyCallbackAttribute(string callbackMemberReferencePath) => CallbackMemberReferencePath = callbackMemberReferencePath;


	/// <summary>
	/// Indicates the path of referenced member.
	/// </summary>
	public string CallbackMemberReferencePath { get; }
}
