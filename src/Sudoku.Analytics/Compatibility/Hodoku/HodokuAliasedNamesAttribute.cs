namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Defines an attribute that is applied to a field of technique,
/// indicating the aliased name (or names) of specified technique that is defined by Hodoku.
/// </summary>
/// <param name="aliases">Indicates the aliased names of the technique.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class HodokuAliasedNamesAttribute([RecordParameter] params string[] aliases) : Attribute;
