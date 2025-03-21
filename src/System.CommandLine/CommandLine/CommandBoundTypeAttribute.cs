namespace System.CommandLine;

/// <summary>
/// Indicates the command-bound types. The type must contain a parameterless constructor with <see langword="public"/> accessibility.
/// </summary>
/// <param name="names">Indicates the names.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed partial class CommandBoundTypeAttribute([Property] params string[] names) : Attribute
{
	/// <summary>
	/// Indicates the conditional property setter values.
	/// </summary>
	public object?[]? ConditionalPropertySetterValues { get; init; }
}
