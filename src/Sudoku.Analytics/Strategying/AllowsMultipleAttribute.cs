namespace Sudoku.Strategying;

/// <summary>
/// Represents an attribute type that determines whether the constraint type supports multiple usages.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AllowsMultipleAttribute : ConstraintAttribute;
