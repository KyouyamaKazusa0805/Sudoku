namespace Sudoku.Analytics.Preferring;

using static SingleSubtype;

/// <summary>
/// Represents a list of preferring usages on <see cref="SingleSubtype"/>.
/// </summary>
/// <seealso cref="SingleSubtype"/>
public static class SingleSubtypePreferring
{
	/// <summary>
	/// Indicates the values.
	/// </summary>
	public static readonly FrozenDictionary<PlayerOperationPrefer, SingleSubtypeGroup> Values = new Dictionary<PlayerOperationPrefer, SingleSubtypeGroup>
	{
		{
			PlayerOperationPrefer.PreferCellClicking,
			[
				FullHouseBlock,
				BlockHiddenSingle011,
				FullHouseRow,
				FullHouseColumn,
				BlockHiddenSingle001,
				BlockHiddenSingle010,
				BlockHiddenSingle012,
				BlockHiddenSingle002,
				BlockHiddenSingle021,
				BlockHiddenSingle020,
				BlockHiddenSingle022,
				RowHiddenSingle001,
				NakedSingleBlock7,
				ColumnHiddenSingle010,
				NakedSingleRow7,
				RowHiddenSingle101,
				ColumnHiddenSingle110,
				RowHiddenSingle102,
				NakedSingleColumn7,
				ColumnHiddenSingle120,
				RowHiddenSingle100,
				ColumnHiddenSingle100,
				RowHiddenSingle002,
				ColumnHiddenSingle020,
				NakedSingleBlock6,
				NakedSingleRow6,
				RowHiddenSingle103,
				NakedSingleColumn6,
				RowHiddenSingle003,
				ColumnHiddenSingle030,
				ColumnHiddenSingle130,
				NakedSingleBlock5,
				RowHiddenSingle004,
				NakedSingleRow5,
				ColumnHiddenSingle040,
				NakedSingleColumn5,
				NakedSingleBlock4,
				RowHiddenSingle104,
				RowHiddenSingle005,
				ColumnHiddenSingle140,
				ColumnHiddenSingle050,
				NakedSingleRow4,
				NakedSingleColumn4,
				RowHiddenSingle006,
				ColumnHiddenSingle060,
				NakedSingleBlock3
			]
		},
		{
			PlayerOperationPrefer.ImitatingLocking,
			[
				BlockHiddenSingle011,
				FullHouseBlock,
				BlockHiddenSingle012,
				BlockHiddenSingle021,
				BlockHiddenSingle001,
				BlockHiddenSingle010,
				BlockHiddenSingle020,
				FullHouseRow,
				FullHouseColumn,
				BlockHiddenSingle022,
				RowHiddenSingle101,
				RowHiddenSingle102,
				ColumnHiddenSingle110,
				ColumnHiddenSingle120,
				RowHiddenSingle001,
				ColumnHiddenSingle010,
				RowHiddenSingle002,
				ColumnHiddenSingle020,
				RowHiddenSingle100,
				ColumnHiddenSingle100,
				RowHiddenSingle103,
				NakedSingleRow7,
				RowHiddenSingle003,
				ColumnHiddenSingle130,
				ColumnHiddenSingle030,
				NakedSingleBlock7,
				NakedSingleColumn7,
				NakedSingleBlock6,
				NakedSingleRow6,
				RowHiddenSingle004,
				ColumnHiddenSingle040,
				NakedSingleColumn6,
				NakedSingleBlock5,
				NakedSingleRow5,
				RowHiddenSingle005,
				RowHiddenSingle104,
				NakedSingleColumn5,
				ColumnHiddenSingle050,
				ColumnHiddenSingle140,
				NakedSingleBlock4,
				RowHiddenSingle006,
				NakedSingleRow4,
				ColumnHiddenSingle060
			]
		},
		{
			PlayerOperationPrefer.PreferDigitLocking,
			[
				BlockHiddenSingle011,
				FullHouseBlock,
				BlockHiddenSingle012,
				BlockHiddenSingle021,
				BlockHiddenSingle002,
				BlockHiddenSingle020,
				BlockHiddenSingle001,
				BlockHiddenSingle010,
				FullHouseRow,
				FullHouseColumn,
				BlockHiddenSingle022,
				RowHiddenSingle001,
				RowHiddenSingle101,
				ColumnHiddenSingle110,
				ColumnHiddenSingle010,
				RowHiddenSingle102,
				ColumnHiddenSingle120,
				NakedSingleBlock7,
				RowHiddenSingle100,
				NakedSingleRow7,
				ColumnHiddenSingle100,
				RowHiddenSingle002,
				ColumnHiddenSingle020,
				NakedSingleColumn7,
				NakedSingleBlock6,
				RowHiddenSingle103,
				RowHiddenSingle003,
				ColumnHiddenSingle030,
				NakedSingleRow6,
				ColumnHiddenSingle130,
				NakedSingleColumn6,
				RowHiddenSingle004,
				ColumnHiddenSingle040,
				NakedSingleBlock5,
				NakedSingleRow5,
				NakedSingleColumn5,
				RowHiddenSingle005,
				RowHiddenSingle104,
				ColumnHiddenSingle140,
				ColumnHiddenSingle050,
				NakedSingleBlock4,
				NakedSingleRow4
			]
		}
	}.ToFrozenDictionary();
}