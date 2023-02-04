namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is used for an auto-notifying field, to define the customized callback method name.
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class NotifyCallbackAttribute : Attribute
{
	/// <summary>
	/// <para>Initializes a <see cref="NotifyCallbackAttribute"/> instance.</para>
	/// <para>
	/// If the callback method ends with string <c>SetterAfter</c> and starts with the property name,
	/// the method will be automatically referenced to the source generation if the method parameters are compatible with the basic rule
	/// of source generation condition.
	/// </para>
	/// </summary>
	public NotifyCallbackAttribute()
	{
	}

	/// <summary>
	/// Initializes a <see cref="NotifyCallbackAttribute"/> instance via the specified path whose corresponding member will be referenced.
	/// </summary>
	/// <param name="callbackMemberReferencePath">The referenced member path.</param>
	public NotifyCallbackAttribute([SuppressMessage("Style", IDE0060, Justification = Pending)] string callbackMemberReferencePath)
	{
	}
}
