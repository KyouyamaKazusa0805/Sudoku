namespace System.Text.RegularExpressions;

/// <summary>
/// Indicates a data member is a regular expression value that represents using a <see cref="string"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class RegexAttribute : Attribute
{
}
