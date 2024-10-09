namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Indicates the backing generated member of this primary constructor is a field.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
public sealed class FieldAttribute : ParameterTargetAttribute
{
	/// <summary>
	/// Indicates whether the generated field or property is implicitly read-only.
	/// If the property is <see langword="true"/>, the generated data member (auto-impl'ed propertys or fields) will be modified
	/// by keyword <see langword="readonly"/> if all following conditions are true:
	/// <list type="number">
	/// <item>
	/// The type is a <see langword="struct"/>, <see langword="record struct"/>,
	/// <see langword="extension"/> or <see langword="role"/> (will be included in future C# version)
	/// </item>
	/// <item>The type is not marked with keyword <see langword="readonly"/></item>
	/// </list>
	/// However, sometimes we should use non-<see langword="readonly"/> <see langword="struct"/> member as fields or auto-impl'ed properties,
	/// but we cannot modify it. By setting the property with <see langword="false"/> value,
	/// to avoid the source generator marking the generated member as <see langword="readonly"/>.
	/// </summary>
	/// <remarks>This property is <see langword="true"/> by default.</remarks>
	public bool IsReadOnlyByDefault { get; init; }
}
