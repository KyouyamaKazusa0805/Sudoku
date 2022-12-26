namespace Sudoku.Solving.Logical.Annotations;

/// <summary>
/// Defines an attribute that is used for <see cref="IStepSearcher"/> derived types,
/// to tell the runtime that the extra information on controlling running operation on <see cref="LogicalSolver"/>.
/// </summary>
/// <seealso cref="IStepSearcher"/>
/// <seealso cref="LogicalSolver"/>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class StepSearcherRunningOptionsAttribute : Attribute
{
	/// <summary>
	/// Initializes a <see cref="StepSearcherRunningOptionsAttribute"/> instance via the specified options.
	/// </summary>
	/// <param name="options">The options.</param>
	public StepSearcherRunningOptionsAttribute(StepSearcherRunningOptions options) => Options = options;


	/// <summary>
	/// Indicates the options.
	/// </summary>
	public StepSearcherRunningOptions Options { get; }
}
