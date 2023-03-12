namespace Sudoku.Platforms.QQ.Modules.Group;

/// <summary>
/// Represents a default value attribute.
/// </summary>
/// <typeparam name="T">The type of the default value. This type argument specified should be same as property type.</typeparam>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class DefaultValueAttribute<T> : CommandLineParsingItemAttribute
{
	/// <summary>
	/// Initializes a <see cref="DefaultValueAttribute{T}"/> instance via the specified default value.
	/// </summary>
	/// <param name="defaultValue">The default value.</param>
	public DefaultValueAttribute(T? defaultValue) => DefaultValue = defaultValue;


	/// <summary>
	/// Indicates the default value.
	/// </summary>
	public T? DefaultValue { get; }
}
