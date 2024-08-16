namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides with an attribute type that can be applied to properties,
/// telling source generator that will be used in factory methods:
/// <code><![CDATA[
/// public static T AddPropertyName(this T instance, PropertyType propertyName)
/// {
///     instance.PropertyName.Add(propertyName);
///     return instance;
/// }
/// ]]></code>
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class AddPropertyAttribute : FactoryPropertyAttribute
{
	/// <summary>
	/// Indicates whether source generator instance won't emit <see langword="params"/> keyword to modify collection parameter
	/// when <see cref="AllowsMultipleAdding"/> is <see langword="true"/>.
	/// </summary>
	/// <seealso cref="AllowsMultipleAdding"/>
	public bool DisallowsMultipleAddingParamsModifier { get; init; }

	/// <summary>
	/// Indicates whether the generator also generates for multiple-element-adding methods.
	/// </summary>
	public bool AllowsMultipleAdding { get; init; }

	/// <summary>
	/// Indicates the type of multiple adding property name to be generated
	/// when <see cref="AllowsMultipleAdding"/> is <see langword="true"/>.
	/// By default the value is <see langword="typeof"/>(<see cref="ReadOnlySpan{T}"/>).
	/// </summary>
	/// <seealso cref="AllowsMultipleAdding"/>
	/// <seealso cref="ReadOnlySpan{T}"/>
	public Type? MultipleAddingPropertyType { get; init; }
}
