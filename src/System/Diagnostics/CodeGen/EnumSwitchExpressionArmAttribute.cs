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
	public EnumSwitchExpressionArmAttribute(
		[SuppressMessage("Style", IDE0060, Justification = Pending)] string key,
		[SuppressMessage("Style", IDE0060, Justification = Pending)] string value
	)
	{
	}
}
