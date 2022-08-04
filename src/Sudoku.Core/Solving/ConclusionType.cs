namespace Sudoku.Solving;

/// <summary>
/// Provides a conclusion type.
/// </summary>
[EnumSwitchExpressionRoot("GetNotation", MethodDescription = "Gets the notation of the conclusion type.", ThisParameterDescription = "The conclusion type.", ReturnValueDescription = "The notation of the conclusion type.")]
public enum ConclusionType : byte
{
	/// <summary>
	/// Indicates the conclusion is a value filling into a cell.
	/// </summary>
	[EnumSwitchExpressionArm("GetNotation", " = ")]
	Assignment,

	/// <summary>
	/// Indicates the conclusion is a candidate being remove from a cell.
	/// </summary>
	[EnumSwitchExpressionArm("GetNotation", " <> ")]
	Elimination
}
