namespace Sudoku.Analytics.Categorization;

/// <summary>
/// Represents a subtype of the single technique set.
/// </summary>
public enum SingleTechniqueSubtype
{
	/// <summary>
	/// Represents the default value of this type.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates the subtype is Full House in Block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "FH1", RelatedTechnique = Technique.FullHouse)]
	FullHouseBlock = 100,

	/// <summary>
	/// Indicates the subtype is Full House in Row.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "FH2", RelatedTechnique = Technique.FullHouse)]
	FullHouseRow = 200,

	/// <summary>
	/// Indicates the subtype is Full House in Column.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "FH3", RelatedTechnique = Technique.FullHouse)]
	FullHouseColumn = 201,

	/// <summary>
	/// Indicates the subtype is Last Digit.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "LD", RelatedTechnique = Technique.LastDigit)]
	LastDigit = 202,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with no excluder values.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHB00", RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle000 = 1001,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (1 row + 1 column).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHB11", RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle011 = 303,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 1 excluder (1 row).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHB10", RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle010 = 301,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 1 excluder (1 column).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHB01", RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle001 = 302,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (2 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHB02", RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle002 = 402,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 2 excluders (2 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHB20", RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle020 = 401,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 3 excluders (2 rows + 1 column).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHB21", RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle021 = 403,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 3 excluders (1 row + 2 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHB12", RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle012 = 404,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Block, with 4 excluders (2 rows + 2 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHB22", RelatedTechnique = Technique.CrosshatchingBlock)]
	BlockHiddenSingle022 = 503,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with no excluders.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR00", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle000 = 1002,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 1 excluder (1 column).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR10", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle001 = 501,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluder (2 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR20", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle002 = 601,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 1 excluder (1 block).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR01", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle100 = 604,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluders (1 block + 1 column).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR11", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle101 = 607,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (3 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR30", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle003 = 613,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (1 block + 2 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR21", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle102 = 615,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (4 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR40", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle004 = 620,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (1 block + 3 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR31", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle103 = 621,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 5 excluders (1 block + 4 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR41", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle104 = 625,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 5 excluders (5 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR50", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle005 = 627,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 6 excluders (6 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR60", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle006 = 629,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 2 excluders (2 blocks).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR02", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle200 = 608,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 3 excluders (2 blocks + 1 column).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR12", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle201 = 609,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Row, with 4 excluders (2 blocks + 2 columns).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHR22", RelatedTechnique = Technique.CrosshatchingRow)]
	RowHiddenSingle202 = 616,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with no excluders.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC00", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle000 = 1003,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 1 excluder (1 row).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC10", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle010 = 502,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (2 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC20", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle020 = 603,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 1 excluder (1 block).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC01", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle100 = 606,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (1 block + 1 row).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC11", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle110 = 610,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (3 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC30", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle030 = 614,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (1 block + 2 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC21", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle120 = 617,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (4 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC40", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle040 = 622,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (1 block + 3 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC31", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle130 = 623,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 5 excluders (5 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC50", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle050 = 628,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 6 excluders (6 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC60", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle060 = 630,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 5 excluders (1 block + 4 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC41", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle140 = 626,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 2 excluders (2 blocks).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC02", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle200 = 611,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 3 excluders (2 blocks + 1 row).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC12", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle210 = 612,

	/// <summary>
	/// Indicates the subtype is Hidden Single in Column, with 4 excluders (2 blocks + 2 rows).
	/// </summary>
	[TechniqueMetadata(Abbreviation = "CHC22", RelatedTechnique = Technique.CrosshatchingColumn)]
	ColumnHiddenSingle220 = 618,

	/// <summary>
	/// Indicates the subtype is Naked Single, with no values in the target block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "NS0", RelatedTechnique = Technique.NakedSingle)]
	NakedSingle0 = 632,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 1 value in the target block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "NS1", RelatedTechnique = Technique.NakedSingle)]
	NakedSingle1 = 631,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 2 values in the target block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "NS2", RelatedTechnique = Technique.NakedSingle)]
	NakedSingle2 = 624,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 3 values in the target block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "NS3", RelatedTechnique = Technique.NakedSingle)]
	NakedSingle3 = 619,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 4 values in the target block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "NS4", RelatedTechnique = Technique.NakedSingle)]
	NakedSingle4 = 605,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 5 values in the target block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "NS5", RelatedTechnique = Technique.NakedSingle)]
	NakedSingle5 = 602,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 6 values in the target block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "NS6", RelatedTechnique = Technique.NakedSingle)]
	NakedSingle6 = 505,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 7 values in the target block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "NS7", RelatedTechnique = Technique.NakedSingle)]
	NakedSingle7 = 504,

	/// <summary>
	/// Indicates the subtype is Naked Single, with 8 values in the target block.
	/// </summary>
	[TechniqueMetadata(Abbreviation = "NS8", RelatedTechnique = Technique.NakedSingle)]
	NakedSingle8 = 1004,

	/// <summary>
	/// Indicates the unknown technique.
	/// </summary>
	Unknown = 2000
}
