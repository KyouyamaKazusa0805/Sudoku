namespace Sudoku.Solving.Manual;

/// <summary>
/// Defines a stableness of a technique.
/// </summary>
public enum Stableness : byte
{
	/// <summary>
	/// Indicates the stableness is stable.
	/// </summary>
	Stable,

	/// <summary>
	/// Indicates the stableness is less unstable.
	/// </summary>
	LessUnstable,

	/// <summary>
	/// Indicates the stableness is unstable.
	/// </summary>
	Unstable,

	/// <summary>
	/// Indicates the stableness is expremely unstable.
	/// </summary>
	ExtremelyUnstable
}
