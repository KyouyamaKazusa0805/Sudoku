namespace Sudoku.Strategying;

/// <summary>
/// Represents an attribute type that determines whether the constraint type supports negation.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AllowsNegationAttribute : ConstraintAttribute;
