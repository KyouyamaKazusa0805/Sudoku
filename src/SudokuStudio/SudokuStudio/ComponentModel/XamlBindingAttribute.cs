namespace SudokuStudio.ComponentModel;

/// <summary>
/// Defines a binding base attribute type.
/// </summary>
/// <typeparam name="T">The type of the property.</typeparam>
public abstract class XamlBindingAttribute<T> : Attribute
{
	/// <summary>
	/// Assigns the value <paramref name="propertyName"/>.
	/// </summary>
	/// <param name="propertyName">The property name.</param>
	protected XamlBindingAttribute([SuppressMessage("Style", IDE0060, Justification = Pending)] string propertyName)
	{
	}


	/// <summary>
	/// Indicates whether the property type is nullable if the member is reference type; for value type, this property has no effect.
	/// </summary>
	public bool IsNullable { get; init; }

	/// <summary>
	/// Indicates the referenced member name that will be used for displaying <c>inheritdoc</c> part.
	/// </summary>
	public string? DocReferencedMemberName { get; init; }

	/// <summary>
	/// Indicates the referenced path that will be used for displaying <c>inheritdoc</c> part.
	/// </summary>
	public string? DocReferencedPath { get; init; }

	/// <summary>
	/// Indicates the <c>summary</c> content of the dependency property displayed in XML documentation comment.
	/// </summary>
	public string? DocSummary { get; init; }

	/// <summary>
	/// Indicates the <c>remarks</c> content of the dependency property displayed in XML documentation comment.
	/// </summary>
	public string? DocRemarks { get; init; }

	/// <summary>
	/// Indicates the referenced member name that points to a member that can create a default value of the current dependency property.
	/// </summary>
	/// <remarks>
	/// This property will be used if the property <see cref="DefaultValue"/> cannot be assigned due to not being a constant.
	/// </remarks>
	/// <seealso cref="DefaultValue"/>
	public string? DefaultValueGeneratingMemberName { get; init; }

	/// <summary>
	/// Indicates the callback method name.
	/// </summary>
	public string? CallbackMethodName { get; init; }

	/// <summary>
	/// Indicates the default value.
	/// </summary>
	public T? DefaultValue { get; init; }
}
