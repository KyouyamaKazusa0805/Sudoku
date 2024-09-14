namespace Sudoku.Generating.Filtering;

/// <summary>
/// Represents an attribute type that describes a constraint usage.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConstraintOptionsAttribute : Attribute
{
	/// <summary>
	/// Indicates whether the constraint can support multiple items defined.
	/// </summary>
	public bool AllowsMultiple { get; init; }

	/// <summary>
	/// Indicates whether the constraint can be negated by user.
	/// </summary>
	public bool AllowsNegation { get; init; }
}
