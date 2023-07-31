namespace Sudoku.Text.Notation;

partial class HobiwanFishNotation
{
	/// <summary>
	/// Represents a kind of the output.
	/// </summary>
	public enum Kind
	{
		/// <summary>
		/// Indicates the notation kind is normal case, which contains the full information of a fish
		/// by using the following expression to combine them:
		/// <code><![CDATA[Digit BaseSet\CoverSet Exofin Endofin]]></code>
		/// For example, if a fish is like the following case:
		/// <code><![CDATA[
		///      c1     c2    c3       c4   c5     c6      c7     c8    c9
		///    ,---------------------,-------------------,--------------------,
		/// r1 | 6      235   12-35  | 345  8      14    | 1357   157   9     |
		/// r2 | 1b358  f358  4      | 7    1b356  9     | 2      158   156   |
		/// r3 | 1-358  7     9      | 356  1356   2     | 1358   4     156   |
		///    :---------------------+-------------------+--------------------:
		/// r4 | 57     1     68     | 9    4567   3     | 4578   2     457   |
		/// r5 | 2      9     68     | 456  14567  1467  | 14578  1578  3     |
		/// r6 | b357   4     b35    | 2    157    8     | 9      6     157   |
		///    :---------------------+-------------------+--------------------:
		/// r7 | 1b345  6     12b35  | 8    b347   47    | 1457   9     12457 |
		/// r8 | 48     28    7      | 1    9      5     | 6      3     24    |
		/// r9 | 9      35    135    | 346  2      467   | 1457   157   8     |
		///    '---------------------'-------------------'--------------------'
		/// ]]></code>
		/// it can be annotated as:
		/// <code><![CDATA[3 r267\c135 fr2c2]]></code>
		/// which means the fish exists with information:
		/// <list type="bullet">
		/// <item>Digit: 3</item>
		/// <item>Base Sets: <c>r2</c>, <c>r6</c> and <c>r7</c></item>
		/// <item>Cover Sets: <c>c1</c>, <c>c3</c> and <c>c5</c></item>
		/// <item>Exo-fin: <c>r2c2</c></item>
		/// </list>
		/// </summary>
		Normal,

		/// <summary>
		/// Indicates the notation kind only outputs for capitals for base and cover sets,
		/// which means only letters <c>r</c>, <c>c</c> and <b>b</b> will be output.
		/// For example, if a fish is like the following case:
		/// <code><![CDATA[
		///      c1     c2    c3       c4   c5     c6      c7     c8    c9
		///    ,---------------------,-------------------,--------------------,
		/// r1 | 6      235   12-35  | 345  8      14    | 1357   157   9     |
		/// r2 | 1b358  f358  4      | 7    1b356  9     | 2      158   156   |
		/// r3 | 1-358  7     9      | 356  1356   2     | 1358   4     156   |
		///    :---------------------+-------------------+--------------------:
		/// r4 | 57     1     68     | 9    4567   3     | 4578   2     457   |
		/// r5 | 2      9     68     | 456  14567  1467  | 14578  1578  3     |
		/// r6 | b357   4     b35    | 2    157    8     | 9      6     157   |
		///    :---------------------+-------------------+--------------------:
		/// r7 | 1b345  6     12b35  | 8    b347   47    | 1457   9     12457 |
		/// r8 | 48     28    7      | 1    9      5     | 6      3     24    |
		/// r9 | 9      35    135    | 346  2      467   | 1457   157   8     |
		///    '---------------------'-------------------'--------------------'
		/// ]]></code>
		/// it can be annotated as:
		/// <code><![CDATA[
		/// rrr\ccc
		/// ]]></code>
		/// There are 3 base sets (i.e. <c>r267</c>) and 3 cover sets (i.e. <c>c135</c>) so both letters <c>r</c> and <c>c</c>
		/// will be repeated 3 times.
		/// </summary>
		CapitalOnly
	}
}
