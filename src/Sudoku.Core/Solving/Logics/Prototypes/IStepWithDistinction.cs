namespace Sudoku.Solving.Prototypes;

/// <summary>
/// Defines a step type that contains the property <c>Distinction Degree</c>.
/// </summary>
public interface IStepWithDistinctionDegree : IStep
{
	/// <summary>
	/// Indicates the distinction degree of the step.
	/// </summary>
	public abstract int DistinctionDegree { get; }
}
