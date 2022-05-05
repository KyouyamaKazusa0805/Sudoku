namespace Sudoku.Solving.Manual;

/// <summary>
/// Defines a stableness of a technique.
/// </summary>
[Flags]
public enum Stableness : byte
{
	/// <summary>
	/// Indicates the stableness is unknown.
	/// </summary>
	Unknown = 0,

	/// <summary>
	/// Indicates the stableness is stable.
	/// </summary>
	Stable = 1,

	/// <summary>
	/// Indicates the stableness is less unstable.
	/// </summary>
	LessUnstable = 1 << 1,

	/// <summary>
	/// Indicates the stableness is unstable.
	/// </summary>
	Unstable = 1 << 2,

	/// <summary>
	/// Indicates the stableness is expremely unstable.
	/// </summary>
	ExtremelyUnstable = 1 << 3
}
