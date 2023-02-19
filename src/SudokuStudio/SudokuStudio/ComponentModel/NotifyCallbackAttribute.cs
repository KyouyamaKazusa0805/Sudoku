namespace SudokuStudio.ComponentModel;

/// <summary>
/// <para>Defines an attribute that is used for an auto-notifying field, to define the customized callback method name.</para>
/// <para>
/// In addition, if you name the callback method "PropertyName<c>SetterAfter</c>" (where "PropertyName" will be replaced with your property),
/// it will be automatically related to source generator, equivalent to expression
/// <see langword="new"/> <see cref="NotifyCallbackAttribute"/>(<see langword="nameof"/>(PropertyName<c>SetterAfter</c>)).
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
[Obsolete($"This type is being deprecated. Please use type '{nameof(CallbackAttribute)}' instead.", false)]
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
