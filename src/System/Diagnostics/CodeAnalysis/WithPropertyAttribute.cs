namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Provides with an attribute type that can be applied to properties,
/// telling source generator that will be used in factory methods:
/// <code><![CDATA[
/// public static T WithPropertyName(this T instance, PropertyType propertyName)
/// {
///     instance.PropertyName = propertyName;
///     return instance;
/// }
/// ]]></code>
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class WithPropertyAttribute : Attribute
{
	/// <summary>
	/// Indicates the parameter modifiers to be applied.
	/// </summary>
	public string? ParameterModifiers { get; init; }

	/// <summary>
	/// Indicates the parameter name to be applied.
	/// </summary>
	public string? ParameterName { get; init; }

	/// <summary>
	/// Indicates the method suffix name to be created.
	/// </summary>
	public string? MethodSuffixName { get; init; }

	/// <summary>
	/// Indicates the accessibility of the factory method generated. By default the accessibility is same as property's.
	/// </summary>
	public string? Accessibility { get; init; }

	/// <summary>
	/// Indicates the parameter type to be created.
	/// </summary>
	public Type? ParameterType { get; init; }
}
