namespace Sudoku.UI;

/// <summary>
/// Defines an attribute that can be used for a property, to indicate the property is read-only
/// but also used by one-way binding operations.
/// </summary>
/// <remarks><b><i>
/// This attribute doesn't make any sense for both the compiler and runtime at present,
/// but this type is still reserved. I may create an analyzer to use this attribute in the future.
/// </i></b></remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
internal sealed class OneWayGetOnlyPropertyAttribute : Attribute
{
}
