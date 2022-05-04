namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that can be applied to a field in an <see cref="Enum"/> type,
/// indicating the source generator will generate the <see langword="switch"/> expression arm
/// for this field, with binding a key-value pair for the generation.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed class EnumSwitchExpressionArmAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="EnumSwitchExpressionArmAttribute"/> instance via the specified value.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	public EnumSwitchExpressionArmAttribute(string key, string value) => (Key, Value) = (key, value);


	/// <summary>
	/// Indicates the key of the <see langword="switch"/> expression arm.
	/// </summary>
	public string Key { get; }

	/// <summary>
	/// Indicates the value of the <see langword="switch"/> expression arm.
	/// </summary>
	public string Value { get; }
}
