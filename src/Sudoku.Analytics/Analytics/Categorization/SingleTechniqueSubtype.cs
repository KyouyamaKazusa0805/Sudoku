namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents a subtype of the single technique set.
/// </summary>
public enum SingleTechniqueSubtype
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
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock, ExtraArguments = [0])]
	BlockHiddenSingle000,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (1 row + 1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock, ExtraArguments = [2])]
	BlockHiddenSingle011,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 1 excluder (1 row).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock, ExtraArguments = [1])]
	BlockHiddenSingle010,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 1 excluder (1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock, ExtraArguments = [1])]
	BlockHiddenSingle001,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock, ExtraArguments = [2])]
	BlockHiddenSingle002,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (2 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock, ExtraArguments = [2])]
	BlockHiddenSingle020,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 3 excluders (2 rows + 1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock, ExtraArguments = [3])]
	BlockHiddenSingle021,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 3 excluders (1 row + 2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock, ExtraArguments = [3])]
	BlockHiddenSingle012,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 4 excluders (2 rows + 2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingBlock, ExtraArguments = [4])]
	BlockHiddenSingle022,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with no excluders.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [0])]
	RowHiddenSingle000,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 1 excluder (1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [1])]
	RowHiddenSingle001,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluder (2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [2])]
	RowHiddenSingle002,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 1 excluder (1 block).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [1])]
	RowHiddenSingle100,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluders (1 block + 1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [2])]
	RowHiddenSingle101,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (3 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [3])]
	RowHiddenSingle003,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (1 block + 2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [3])]
	RowHiddenSingle102,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (4 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [4])]
	RowHiddenSingle004,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (1 block + 3 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [4])]
	RowHiddenSingle103,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 5 excluders (1 block + 4 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [5])]
	RowHiddenSingle104,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 5 excluders (5 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [5])]
	RowHiddenSingle005,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 6 excluders (6 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [6])]
	RowHiddenSingle006,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluders (2 blocks).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [2])]
	RowHiddenSingle200,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (2 blocks + 1 column).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [3])]
	RowHiddenSingle201,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (2 blocks + 2 columns).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingRow, ExtraArguments = [4])]
	RowHiddenSingle202,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with no excluders.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [0])]
	ColumnHiddenSingle000,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 1 excluder (1 row).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [1])]
	ColumnHiddenSingle010,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (2 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [2])]
	ColumnHiddenSingle020,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 1 excluder (1 block).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [1])]
	ColumnHiddenSingle100,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (1 block + 1 row).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [2])]
	ColumnHiddenSingle110,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (3 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [3])]
	ColumnHiddenSingle030,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (1 block + 2 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [3])]
	ColumnHiddenSingle120,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (4 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [4])]
	ColumnHiddenSingle040,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (1 block + 3 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [4])]
	ColumnHiddenSingle130,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 5 excluders (5 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [5])]
	ColumnHiddenSingle050,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 6 excluders (6 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [6])]
	ColumnHiddenSingle060,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 5 excluders (1 block + 4 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [5])]
	ColumnHiddenSingle140,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (2 blocks).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [2])]
	ColumnHiddenSingle200,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (2 blocks + 1 row).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [3])]
	ColumnHiddenSingle210,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (2 blocks + 2 rows).
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.CrosshatchingColumn, ExtraArguments = [4])]
	ColumnHiddenSingle220,

	/// <summary>
	/// Indicates the subtype is Naked Single, with no values in the target block.
	/// </summary>
	[TechniqueMetadata(RelatedTechnique = Technique.NakedSingle, ExtraArguments = [0])]
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
