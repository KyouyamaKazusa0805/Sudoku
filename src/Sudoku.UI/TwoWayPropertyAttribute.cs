namespace Sudoku.UI;

/// <summary>
/// Defines an attribute that can be used for a property, to indicate the property contains both
/// setter and getter, and used by two-way binding operations, with the specified property name.
/// </summary>
/// <remarks><b><i>
/// This attribute doesn't make any sense for both the compiler and runtime at present,
/// but this type is still reserved. I may create an analyzer to use this attribute in the future.
/// </i></b></remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
internal sealed class TwoWayPropertyAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="TwoWayPropertyAttribute"/> instance via the specified property name
	/// that is the bound value with the current instance.
	/// </summary>
	/// <param name="boundPropertyName">The name of that bound property.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TwoWayPropertyAttribute(string boundPropertyName) => BoundPropertyName = boundPropertyName;


	/// <summary>
	/// Indicates the bound property name.
	/// </summary>
	public string BoundPropertyName { get; }
}
