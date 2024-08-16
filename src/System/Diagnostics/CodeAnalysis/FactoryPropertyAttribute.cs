namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a factory property attribute.
/// </summary>
public abstract class FactoryPropertyAttribute : Attribute
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
