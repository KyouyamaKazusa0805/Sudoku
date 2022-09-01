namespace Sudoku.Text;

/// <summary>
/// Marks on a method to tell the user the method is only used for the formatting.
/// This attribute implies two things:
/// <list type="number">
/// <item>The name of the method is appeared in resource dictionary</item>
/// <item>The method can only be invoked by reflection.</item>
/// </list>
/// If a method is marked this attribute, please also applies <see cref="DynamicDependencyAttribute"/>
/// and <see cref="DynamicallyAccessedMembersAttribute"/>.
/// </summary>
/// <seealso cref="DynamicDependencyAttribute"/>
/// <seealso cref="DynamicallyAccessedMembersAttribute"/>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ResourceTextFormatterAttribute : Attribute
{
}
