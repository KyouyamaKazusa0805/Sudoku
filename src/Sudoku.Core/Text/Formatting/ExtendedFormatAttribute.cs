namespace Sudoku.Text.Formatting;

/// <summary>
/// Indicates the extended format.
/// </summary>
/// <param name="format">Indicates the extended format.</param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed partial class ExtendedFormatAttribute([DataMember] string format) : Attribute;
