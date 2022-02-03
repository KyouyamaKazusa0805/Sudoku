namespace Sudoku.Solving.Manual.Text;

/// <summary>
/// Marks on a property to tell the user the property is only used for the formatting.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class FormatItemAttribute : Attribute
{
}
