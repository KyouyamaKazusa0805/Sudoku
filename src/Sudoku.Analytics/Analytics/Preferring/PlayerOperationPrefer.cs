namespace Sudoku.Analytics.Preferring;

/// <summary>
/// Represents a kind of the behavior that specifies which way a player may prefer.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
[Flags]
public enum PlayerOperationPrefer
{
	/// <summary>
	/// Indicates the preferred behavior is unknown.
	/// </summary>
	Unknown = 0,

	/// <summary>
	/// Indicates a user like clicking a cell, and then click digit button to fill with that.
	/// </summary>
	PreferCellClicking = 1 << 0,

	/// <summary>
	/// Indicates a user like locking digits, and fill digits with only clicking cells.
	/// </summary>
	PreferDigitLocking = 1 << 1,

	/// <summary>
	/// Indicates a user like nearest path to fill digits.
	/// </summary>
	PreferNearestPathingCell = 1 << 2,

	/// <summary>
	/// Indicates a user like filling with digits appeared more than the others.
	/// </summary>
	PreferLeastLastingDigit = 1 << 3,

	/// <summary>
	/// Indicates a user like filling with cells in a block that contains less empty cells with other blocks.
	/// </summary>
	PreferLeastLastingBlock = 1 << 4,

	/// <summary>
	/// Indicates a user like filling with digit ittoryu mode.
	/// </summary>
	PreferDigitIttoryu = 1 << 5,

	/// <summary>
	/// Indicates a user like filling with house ittoryu mode.
	/// </summary>
	PreferHouseIttoryu = 1 << 6,

	/// <summary>
	/// Indicates a user like filling with cell guessing.
	/// </summary>
	CellGuessing = 1 << 7,

	/// <summary>
	/// Indicates a user like filling with house guessing.
	/// </summary>
	HouseGuessing = 1 << 8,

	/// <summary>
	/// Indicates a user like filling with guessing without any order.
	/// </summary>
	RandomGuessing = 1 << 9,

	/// <summary>
	/// Indicates a user like filling digits by imitating locking.
	/// </summary>
	ImitatingLocking = 1 << 10
}
