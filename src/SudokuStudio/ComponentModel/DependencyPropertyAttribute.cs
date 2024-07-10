namespace SudokuStudio.ComponentModel;

/// <summary>
/// Defines an attribute that defines a dependency property.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class DependencyPropertyAttribute : Attribute
{
	/// <summary>
	/// Indicates the default value.
	/// </summary>
	public object? DefaultValue { get; init; }
}
