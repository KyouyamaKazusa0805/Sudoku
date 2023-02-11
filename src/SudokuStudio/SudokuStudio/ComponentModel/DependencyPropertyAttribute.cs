namespace SudokuStudio.ComponentModel;

/// <summary>
/// Defines an attribute that defines a dependency property.
/// </summary>
/// <typeparam name="T">Indicates the type of the property evaluated.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DependencyPropertyAttribute<T> : XamlBindingAttribute<T>
{
	/// <summary>
	/// Initializes a <see cref="DependencyPropertyAttribute{T}"/> instance via the generated property name.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	public DependencyPropertyAttribute(string propertyName) : base(propertyName)
	{
	}
}
