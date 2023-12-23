namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents an attribute type that describes its abbreviation of a technique group.
/// </summary>
/// <param name="abbr">Indicates the abbreviation.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class AbbreviationAttribute([Data(GeneratedMemberName = "Abbreviation")] string abbr) : Attribute;
