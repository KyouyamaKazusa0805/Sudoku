namespace Sudoku.Solving;

/// <summary>
/// Indicates the runtime solver names.
/// </summary>
/// <param name="names">Indicates the names.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed partial class RuntimeSolverConfigurationAttribute([Property] params string[] names) : Attribute
{
	/// <summary>
	/// Indicates the conditional property setter values.
	/// </summary>
	public object?[]? ConditionalPropertySetterValues { get; init; }
}
