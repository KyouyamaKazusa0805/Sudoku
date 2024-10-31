namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a base technique of multiple forcing chains.
/// </summary>
public enum MultipleChainBasedComponent
{
	/// <summary>
	/// Indicates the multiple forcing chains starts with a cell.
	/// </summary>
	Cell,

	/// <summary>
	/// Indicates the multiple forcing chains starts with a house.
	/// </summary>
	House,

	/// <summary>
	/// Indicates the multiple forcing chains starts with a unique rectangle.
	/// </summary>
	Rectangle
}
