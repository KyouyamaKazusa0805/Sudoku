namespace SudokuStudio.ComponentModel;

/// <summary>
/// Defines an attribute that defines a dependency property.
/// </summary>
/// <param name="propertyName"><inheritdoc/></param>
/// <typeparam name="T">Indicates the type of the property evaluated.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DependencyPropertyAttribute<T>([StringSyntax(StringSyntax.Regex)] string propertyName) : XamlBindingAttribute<T>(propertyName)
{
	/// <summary>
	/// Indicates whether the dependency property returns <see langword="true"/>,
	/// the specified target members won't be <see langword="null"/>.
	/// </summary>
	public string[]? MembersNotNullWhenReturnsTrue { get; init; }

	/// <summary>
	/// Indicates the property accessibility. The default value is <see cref="Accessibility.Public"/>.
	/// </summary>
	/// <remarks>
	/// This property only works with dependency properties.
	/// </remarks>
	/// <seealso cref="Accessibility.Public"/>
	public Accessibility Accessibility { get; init; }
}
