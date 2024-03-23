namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents a subtype of the single technique set.
/// </summary>
public enum SingleSubtype
{
	/// <summary>
	/// Indicates the subtype is Full House in Block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.FullHouse)]
	FullHouseBlock,

	/// <summary>
	/// Indicates the subtype is Full House in Row.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.FullHouse)]
	FullHouseRow,

	/// <summary>
	/// Indicates the subtype is Full House in Column.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.FullHouse)]
	FullHouseColumn,

	/// <summary>
	/// Indicates the subtype is Last Digit.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.LastDigit)]
	LastDigit,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with no excluder values.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle000,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (1 row + 1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle011,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 1 excluder (1 row).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle010,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 1 excluder (1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle001,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle002,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (2 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle020,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 3 excluders (2 rows + 1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle021,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 3 excluders (1 row + 2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle012,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 4 excluders (2 rows + 2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle022,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with no excluders.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle000,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 1 excluder (1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle001,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluder (2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle002,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 1 excluder (1 block).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle100,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluders (1 block + 1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle101,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (3 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle003,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (1 block + 2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle102,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (4 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle004,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (1 block + 3 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle103,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 5 excluders (1 block + 4 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle104,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 5 excluders (5 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle005,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 6 excluders (6 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle006,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluders (2 blocks).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle200,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (2 blocks + 1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle201,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (2 blocks + 2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle202,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with no excluders.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle000,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 1 excluder (1 row).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle010,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (2 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle020,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 1 excluder (1 block).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle100,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (1 block + 1 row).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle110,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (3 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle030,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (1 block + 2 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle120,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (4 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle040,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (1 block + 3 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle130,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 5 excluders (5 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle050,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 6 excluders (6 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle060,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 5 excluders (1 block + 4 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle140,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (2 blocks).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle200,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (2 blocks + 1 row).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle210,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (2 blocks + 2 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle220,

	/// <summary>
	/// Indicates the subtype is Naked Single, with no values in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle)]
	NakedSingle0,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 1 value in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle)]
	NakedSingle1,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 2 values in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle)]
	NakedSingle2,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 3 values in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle)]
	NakedSingle3,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 4 values in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle)]
	NakedSingle4,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 5 values in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle)]
	NakedSingle5,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 6 values in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle)]
	NakedSingle6,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 7 values in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle)]
	NakedSingle7,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 8 values in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle)]
	NakedSingle8
}
