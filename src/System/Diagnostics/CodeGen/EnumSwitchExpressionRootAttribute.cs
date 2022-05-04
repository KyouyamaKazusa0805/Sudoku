namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that can be applied to an <see cref="Enum"/> type,
/// indicating the source generator will generate the code for routing enumeration fields.
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = true, Inherited = false)]
public sealed class EnumSwitchExpressionRootAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="EnumSwitchExpressionRootAttribute"/> instance via the key.
	/// </summary>
	/// <param name="key">The key.</param>
	public EnumSwitchExpressionRootAttribute(string key) => Key = key;


	/// <summary>
	/// Indicates the name of the switch expression root.
	/// </summary>
	public string Key { get; }

	/// <summary>
	/// Indicates the error case that the specified attribute cannot be found in an enumeration field.
	/// The default value is <see cref="EnumSwitchExpressionNotDefinedBehavior.ThrowForNotDefined"/>.
	/// </summary>
	public EnumSwitchExpressionNotDefinedBehavior NotDefinedBehavior { get; init; }
		= EnumSwitchExpressionNotDefinedBehavior.ThrowForNotDefined;
}
