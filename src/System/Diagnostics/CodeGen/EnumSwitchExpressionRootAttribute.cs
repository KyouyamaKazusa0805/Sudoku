namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an <see langword="enum"/> field as a root in <see langword="switch"/> expression.
/// </summary>
[AttributeUsage(AttributeTargets.Enum, AllowMultiple = true, Inherited = false)]
public sealed class EnumSwitchExpressionRootAttribute : Attribute
{
	/// <summary>
	/// Initializes an <see cref="EnumSwitchExpressionRootAttribute"/> instance.
	/// </summary>
	/// <param name="key">The key.</param>
	public EnumSwitchExpressionRootAttribute(string key) => Key = key;


	/// <summary>
	/// Indicates the method description that is used to be put into <c>summary</c> part in doc comments.
	/// </summary>
	public string? MethodDescription { get; init; }

	/// <summary>
	/// Indicates the method description that is used to be put into <c>param name="@this"</c> part in doc comments.
	/// </summary>
	public string? ThisParameterDescription { get; init; }

	/// <summary>
	/// Indicates the method description that is used to be put into <c>returns</c> part in doc comments.
	/// </summary>
	public string? ReturnValueDescription { get; init; }

	/// <summary>
	/// Indicates the key.
	/// </summary>
	public string Key { get; }

	/// <summary>
	/// Indicates the default behavior for default cases.
	/// </summary>
	public EnumSwitchExpressionDefaultBehavior DefaultBehavior { get; init; } = EnumSwitchExpressionDefaultBehavior.Throw;
}
