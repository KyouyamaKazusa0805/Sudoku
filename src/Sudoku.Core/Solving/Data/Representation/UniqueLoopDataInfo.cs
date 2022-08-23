namespace Sudoku.Solving.Data.Representation;

/// <summary>
/// Represents for a data set that describes the complete information about a unique loop technique.
/// </summary>
/// <param name="Loop">Indicates the cells used in this whole guardian loop.</param>
/// <param name="Digits">Indicates the digits used.</param>
public readonly record struct UniqueLoopDataInfo(scoped in Cells Loop, short Digits) :
	IEquatable<UniqueLoopDataInfo>,
	IEqualityOperators<UniqueLoopDataInfo, UniqueLoopDataInfo>,
	ITechniqueDataInfo<UniqueLoopDataInfo>
{
	/// <inheritdoc/>
	Cells ITechniqueDataInfo<UniqueLoopDataInfo>.Map => Loop;
}
