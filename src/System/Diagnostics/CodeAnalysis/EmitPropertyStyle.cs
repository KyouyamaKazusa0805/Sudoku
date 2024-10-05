namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents a style of emit property.
/// </summary>
public enum EmitPropertyStyle
{
	/// <summary>
	/// Indicates the behavior is to generate an assignment to property:
	/// <code><![CDATA[public int Property { get; } = value;]]></code>
	/// </summary>
	AssignToProperty,

	/// <summary>
	/// Indicates the behavior is to generate a return statement that directly returns parameter:
	/// <code><![CDATA[public int Property => value;]]></code>
	/// </summary>
	ReturnParameter
}
