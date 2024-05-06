namespace Sudoku.Measuring;

/// <summary>
/// Represents an attribute type that describes the member is a <see langword="static"/> property,
/// returning a <see cref="string"/> whose internal value is the raw C# code of member function.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed partial class ExportedMemberAttribute : Attribute;
