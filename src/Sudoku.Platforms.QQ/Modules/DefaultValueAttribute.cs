namespace Sudoku.Platforms.QQ.Modules;

/// <summary>
/// Represents a default value attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DefaultValueAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="DefaultValueAttribute"/> instance via the specified default value invoker name.
	/// </summary>
	/// <param name="defaultValueInvokerName">
	/// <para>The default value invoker name.</para>
	/// <para><inheritdoc cref="DefaultValueInvokerName" path="/remarks"/></para>
	/// </param>
	public DefaultValueAttribute(string defaultValueInvokerName) => DefaultValueInvokerName = defaultValueInvokerName;


	/// <summary>
	/// Indicates the default value invoker name.
	/// </summary>
	/// <remarks><b><i>
	/// The referenced member can only be <see langword="static"/> fields, <see langword="static"/> properties
	/// or <see langword="static"/> parameterless methods. Methods cannot be local functions or other methods
	/// whose name cannot be known in compiler time.
	/// </i></b></remarks>
	public string DefaultValueInvokerName { get; }
}
