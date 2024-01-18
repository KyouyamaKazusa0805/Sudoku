namespace Sudoku.Concepts;

/// <summary>
/// Represents a subtype of the single technique.
/// </summary>
public enum SingleSubtype
{
	/// <summary>
	/// Indicates the subtype is Full House in Block.
	/// </summary>
	FullHouseBlock,

	/// <summary>
	/// Indicates the subtype is Full House in Row.
	/// </summary>
	FullHouseRow,

	/// <summary>
	/// Indicates the subtype is Full House in Column.
	/// </summary>
	FullHouseColumn,

	/// <summary>
	/// Indicates the subtype is Last Digit.
	/// </summary>
	LastDigit,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with no excluder values.
	/// </summary>
	BlockHiddenSingle000,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (1 row + 1 column).
	/// </summary>
	BlockHiddenSingle011,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 1 excluder (1 row).
	/// </summary>
	BlockHiddenSingle010,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 1 excluder (1 column).
	/// </summary>
	BlockHiddenSingle001,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (2 columns).
	/// </summary>
	BlockHiddenSingle002,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (2 rows).
	/// </summary>
	BlockHiddenSingle020,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 3 excluders (2 rows + 1 column).
	/// </summary>
	BlockHiddenSingle021,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 3 excluders (1 row + 2 columns).
	/// </summary>
	BlockHiddenSingle012,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 4 excluders (2 rows + 2 columns).
	/// </summary>
	BlockHiddenSingle022,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with no excluders.
	/// </summary>
	RowHiddenSingle000,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 1 excluder (1 column).
	/// </summary>
	RowHiddenSingle001,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluder (2 columns).
	/// </summary>
	RowHiddenSingle002,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 1 excluder (1 block).
	/// </summary>
	RowHiddenSingle100,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluders (1 block + 1 column).
	/// </summary>
	RowHiddenSingle101,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (3 columns).
	/// </summary>
	RowHiddenSingle003,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (1 block + 2 columns).
	/// </summary>
	RowHiddenSingle102,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (4 columns).
	/// </summary>
	RowHiddenSingle004,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (1 block + 3 columns).
	/// </summary>
	RowHiddenSingle103,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 5 excluders (1 block + 4 columns).
	/// </summary>
	RowHiddenSingle104,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 5 excluders (5 columns).
	/// </summary>
	RowHiddenSingle005,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 6 excluders (6 columns).
	/// </summary>
	RowHiddenSingle006,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluders (2 blocks).
	/// </summary>
	RowHiddenSingle200,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (2 blocks + 1 column).
	/// </summary>
	RowHiddenSingle201,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (2 blocks + 2 columns).
	/// </summary>
	RowHiddenSingle202,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with no excluders.
	/// </summary>
	ColumnHiddenSingle000,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 1 excluder (1 row).
	/// </summary>
	ColumnHiddenSingle010,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (2 rows).
	/// </summary>
	ColumnHiddenSingle020,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 1 excluder (1 block).
	/// </summary>
	ColumnHiddenSingle100,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (1 block + 1 row).
	/// </summary>
	ColumnHiddenSingle110,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (3 rows).
	/// </summary>
	ColumnHiddenSingle030,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (1 block + 2 rows).
	/// </summary>
	ColumnHiddenSingle120,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (4 rows).
	/// </summary>
	ColumnHiddenSingle040,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (1 block + 3 rows).
	/// </summary>
	ColumnHiddenSingle130,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 5 excluders (5 rows).
	/// </summary>
	ColumnHiddenSingle050,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 6 excluders (6 rows).
	/// </summary>
	ColumnHiddenSingle060,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 5 excluders (1 block + 4 rows).
	/// </summary>
	ColumnHiddenSingle140,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (2 blocks).
	/// </summary>
	ColumnHiddenSingle200,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (2 blocks + 1 row).
	/// </summary>
	ColumnHiddenSingle210,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (2 blocks + 2 rows).
	/// </summary>
	ColumnHiddenSingle220,

	/// <summary>
	/// Indicates the subtype is Naked Single, with no values in the target block.
	/// </summary>
	NakedSingle0,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 1 value in the target block.
	/// </summary>
	NakedSingle1,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 2 values in the target block.
	/// </summary>
	NakedSingle2,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 3 values in the target block.
	/// </summary>
	NakedSingle3,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 4 values in the target block.
	/// </summary>
	NakedSingle4,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 5 values in the target block.
	/// </summary>
	NakedSingle5,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 6 values in the target block.
	/// </summary>
	NakedSingle6,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 7 values in the target block.
	/// </summary>
	NakedSingle7,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 8 values in the target block.
	/// </summary>
	NakedSingle8
}
