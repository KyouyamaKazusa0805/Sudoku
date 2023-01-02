namespace Sudoku.Text;

/// <summary>
/// Marks on a method to tell the user the method is only used for the formatting.
/// This attribute implies two things:
/// <list type="number">
/// <item>The name of the method is appeared in resource dictionary.</item>
/// <item>The method can only be invoked by reflection.</item>
/// </list>
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ResourceTextFormatterAttribute : Attribute
{
}
