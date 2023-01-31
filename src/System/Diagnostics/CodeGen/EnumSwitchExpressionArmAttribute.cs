namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an <see langword="enum"/> field as an arm in <see langword="switch"/> expressions.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
public sealed class EnumSwitchExpressionArmAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="EnumSwitchExpressionArmAttribute"/> instance.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <param name="value">The value.</param>
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	public EnumSwitchExpressionArmAttribute(string key, string value)
	{
	}
}
