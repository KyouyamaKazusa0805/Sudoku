namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that is applied to a type, specifying overloading operator information details.
/// </summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class GeneratedOverloadingOperatorAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="GeneratedOverloadingOperatorAttribute"/> instance via the specified operator name.
	/// </summary>
	/// <param name="operator">The operator name.</param>
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	public GeneratedOverloadingOperatorAttribute(GeneratedOperator @operator)
	{
	}
}
