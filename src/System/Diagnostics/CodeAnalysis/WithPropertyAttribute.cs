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
public sealed class WithPropertyAttribute : FactoryPropertyAttribute;
