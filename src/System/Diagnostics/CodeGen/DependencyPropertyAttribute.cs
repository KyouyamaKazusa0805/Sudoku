namespace System.Diagnostics.CodeGen;

/// <summary>
/// Defines an attribute that defines a dependency property.
/// </summary>
/// <typeparam name="T">Indicates the type of the property evaluated.</typeparam>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class DependencyPropertyAttribute<T> : Attribute
{
	/// <summary>
	/// Initializes a <see cref="DependencyPropertyAttribute{T}"/> instance via the generated property name.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	public DependencyPropertyAttribute([SuppressMessage("Style", IDE0060, Justification = Pending)] string propertyName)
	{
	}


	/// <summary>
	/// Indicates the referenced member name that will be used for displaying <c>inheritdoc</c> part.
	/// </summary>
	public string? DocReferencedMemberName { get; init; }

	/// <summary>
	/// Indicates the referenced path that will be used for displaying <c>inheritdoc</c> part.
	/// </summary>
	public string? DocReferencedPath { get; init; }

	/// <summary>
	/// Indicates the referenced member name that points to a member that can create a default value of the current dependency property.
	/// </summary>
	/// <remarks>
	/// This property will be used if the property <see cref="DefaultValue"/> cannot be assigned due to not being a constant.
	/// </remarks>
	/// <seealso cref="DefaultValue"/>
	public string? DefaultValueGeneratingMemberName { get; init; }

	/// <summary>
	/// Indicates the default value.
	/// </summary>
	public T? DefaultValue { get; init; }
}
