namespace System.Diagnostics.CodeGen;

/// <summary>
/// Indicates user cannot use parameterless constructor for this type.
/// </summary>
[AttributeUsage(AttributeTargets.Struct, Inherited = false)]
public sealed class DisallowParameterlessConstructorAttribute : Attribute
{
	/// <summary>
	/// Indicates the suggested instance that you want to suggest user use.
	/// </summary>
	public string? SuggestedInstanceName { get; init; }
}
