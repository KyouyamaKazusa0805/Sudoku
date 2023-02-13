namespace SudokuStudio.ComponentModel;

/// <summary>
/// Defines an attribute that defines an attached property.
/// </summary>
/// <typeparam name="T">Indicates the type of the property evaluated.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class AttachedPropertyAttribute<T> : XamlBindingAttribute<T>
{
	/// <summary>
	/// Initializes a <see cref="AttachedPropertyAttribute{T}"/> instance via the generated property name.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	public AttachedPropertyAttribute(string propertyName) : base(propertyName)
	{
	}
}
