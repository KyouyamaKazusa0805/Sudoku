namespace Sudoku.Solving.Logics.Implementations.Data;

/// <summary>
/// Represents for a data set that describes the complete information about a unique loop technique.
/// </summary>
/// <param name="Loop">Indicates the cells used in this whole unique loop.</param>
/// <param name="DigitsMask">Indicates the digits used, represented as a mask of type <see cref="short"/>.</param>
public readonly record struct UniqueLoopDataInfo(scoped in CellMap Loop, short DigitsMask) :
	IEquatable<UniqueLoopDataInfo>,
	IEqualityOperators<UniqueLoopDataInfo, UniqueLoopDataInfo, bool>,
	ITechniqueDataInfo<UniqueLoopDataInfo>
{
	/// <inheritdoc/>
	CellMap ITechniqueDataInfo<UniqueLoopDataInfo>.Map => Loop;
}
