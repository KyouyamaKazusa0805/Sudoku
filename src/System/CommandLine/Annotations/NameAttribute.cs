namespace System.CommandLine.Annotations;

/// <summary>
/// Defines an attribute that is applied to an enumeration field, indicating the name of the field.
/// </summary>
/// <param name="name">The name of the field.</param>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class NameAttribute(string name) : Attribute
{
	/// <summary>
	/// Indicates the name of the enumeration field.
	/// </summary>
	public string Name { get; } = name;
}
