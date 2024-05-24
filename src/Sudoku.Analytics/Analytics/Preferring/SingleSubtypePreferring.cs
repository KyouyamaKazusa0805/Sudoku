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
		}
	}.ToFrozenDictionary();
}
