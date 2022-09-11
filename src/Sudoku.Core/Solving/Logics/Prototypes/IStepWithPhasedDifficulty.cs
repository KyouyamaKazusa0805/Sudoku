namespace Sudoku.Solving.Prototypes;

/// <summary>
/// Defines a type that holds an extra property to control advanced rule with extra difficulty value.
/// </summary>
public interface IStepWithPhasedDifficulty
{
	/// <summary>
	/// Indicates the base difficulty of the step.
	/// </summary>
	public abstract decimal BaseDifficulty { get; }

	/// <summary>
	/// Indicates the extra difficulty values of the step, and its corresponding description.
	/// </summary>
	public abstract (string Name, decimal Value)[] ExtraDifficultyValues { get; }

	/// <summary>
	/// Indicates the total difficulty value of the step.
	/// </summary>
	protected internal sealed decimal TotalDifficulty
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => BaseDifficulty + ExtraDifficultyValues.Sum(static s => s.Value);
	}
}
