namespace Sudoku.Solving.Logical.PatternData;

/// <summary>
/// Represents for a data structure that describes for a technique structure.
/// </summary>
/// <typeparam name="T">The type of the implemented data structure.</typeparam>
public interface ITechniqueDataInfo<T> where T : struct, ITechniqueDataInfo<T>
{
	/// <summary>
	/// Indicates the cells used in this whole technique structure.
	/// </summary>
	CellMap Map { get; }
}
