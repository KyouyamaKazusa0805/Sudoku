﻿namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Defines an attribute that is applied to a field of technique,
/// indicating the aliased name (or names) of specified technique that is defined by Hodoku.
/// </summary>
/// <param name="aliases">The aliased names of specified technique.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed class HodokuAliasedNamesAttribute(params string[] aliases) : Attribute
{
	/// <summary>
	/// Indicates the aliased names of the technique.
	/// </summary>
	public string[] Aliases { get; } = aliases;
}
