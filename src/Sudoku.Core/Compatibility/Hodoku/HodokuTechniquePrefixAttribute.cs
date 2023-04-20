namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Defines an attribute that is applied to a field in technique, indicating a compatible prefix value defined by Hodoku.
/// </summary>
/// <param name="prefix">Indicates the prefix value.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class HodokuTechniquePrefixAttribute([PrimaryConstructorParameter] string prefix) : Attribute;
