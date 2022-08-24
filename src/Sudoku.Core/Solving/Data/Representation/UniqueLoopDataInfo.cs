namespace Sudoku.Solving.Data.Representation;

/// <summary>
/// Represents for a data set that describes the complete information about a unique loop technique.
/// </summary>
/// <param name="Loop">Indicates the cells used in this whole unique loop.</param>
/// <param name="DigitsMask">Indicates the digits used, represented as a mask of type <see cref="short"/>.</param>
public readonly record struct UniqueLoopDataInfo(scoped in Cells Loop, short DigitsMask) :
	IEquatable<UniqueLoopDataInfo>,
	IEqualityOperators<UniqueLoopDataInfo, UniqueLoopDataInfo>,
	ITechniqueDataInfo<UniqueLoopDataInfo>
{
	/// <inheritdoc/>
	Cells ITechniqueDataInfo<UniqueLoopDataInfo>.Map => Loop;
}
