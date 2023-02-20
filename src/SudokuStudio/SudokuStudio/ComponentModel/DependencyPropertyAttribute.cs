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


	/// <summary>
	/// Indicates whether the dependency property returns <see langword="true"/>,
	/// the specified target members won't be <see langword="null"/>.
	/// </summary>
	public string[]? MembersNotNullWhenReturnsTrue { get; init; }

	/// <summary>
	/// Indicates the property accessibility. The default value is <see cref="GeneralizedAccessibility.Public"/>.
	/// </summary>
	/// <remarks>
	/// This property only works with dependency properties.
	/// </remarks>
	/// <seealso cref="GeneralizedAccessibility.Public"/>
	public GeneralizedAccessibility Accessibility { get; init; }
}
