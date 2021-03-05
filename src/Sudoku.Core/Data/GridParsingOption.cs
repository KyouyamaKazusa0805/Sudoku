namespace Sudoku.Data
{
	/// <summary>
	/// Represents a grid parsing type.
	/// </summary>
	/*closed*/
	public enum GridParsingOption : byte
	{
		/// <summary>
		/// <para>
		/// Indicates the susser format, which means all grid values
		/// will be displayed in one line with empty cell character
		/// <c>'0'</c> or <c>'.'</c>.
		/// </para>
		/// <para>
		/// For example:
		/// <code>
		/// 080630040200085009090000081000300800000020000006001000970000030400850007010094050
		/// 4+80002+306+31000+9004000+4030109+38+24+675+1+75+1+39+8+46+2+2640708+3902090000010000002080+36+20007:713 723 533 633 537 575 176 576 577 579 583 784 586 587 496 997
		/// </code>
		/// </para>
		/// </summary>
		Susser,

		/// <summary>
		/// <para>
		/// Indicates the pencil marked grid (PM grid), which means all
		/// grid candidates will be displayed using a table.
		/// </para>
		/// <para>
		/// For example:
		/// <code>
		/// +-------------------+-----------------+--------------------+
		/// | &lt;8&gt;   12459  249  | *6*  &lt;7&gt;   259  | 1245    245   *3*  |
		/// | 1457  1245   &lt;6&gt;  | 58   125   &lt;3&gt;  | 124578  2458  &lt;9&gt;  |
		/// | 1579  &lt;3&gt;    279  | &lt;4&gt;  1259  2589 | 12578   *6*   78   |
		/// +-------------------+-----------------+--------------------+
		/// | *2*   &lt;7&gt;    *3*  | &lt;1&gt;  &lt;8&gt;   45   | 456     &lt;9&gt;   46   |
		/// | &lt;6&gt;   458    48   | &lt;2&gt;  59    459  | &lt;3&gt;     *7*   *1*  |
		/// | 1459  1459   49   | *3*  *6*   &lt;7&gt;  | 458     458   &lt;2&gt;  |
		/// +-------------------+-----------------+--------------------+
		/// | &lt;3&gt;   2489   &lt;1&gt;  | 578  25    &lt;6&gt;  | 24789   248   478  |
		/// | 79    &lt;6&gt;    2789 | 78   &lt;4&gt;   *1*  | 289     *3*   &lt;5&gt;  |
		/// | 47    248    &lt;5&gt;  | &lt;9&gt;  *3*   28   | 2678    &lt;1&gt;   4678 |
		/// +-------------------+-----------------+--------------------+
		/// </code>
		/// (Due to the rendering engine of IDE, all spaces will be displayed only once
		/// in this table.)
		/// </para>
		/// </summary>
		PencilMarked,

		/// <summary>
		/// <para>
		/// Indicates the table format, which means all grid values
		/// will be displayed using a table with empty cell character
		/// <c>'0'</c> or <c>'.'</c>.
		/// </para>
		/// <para>
		/// For example:
		/// <code>
		/// .----------.----------.----------.
		/// |  .  4  6 |  . +1  . |  3 +7 +5 |
		/// | +3 +8  1 | +5  4  7 | +2 +9 +6 |
		/// |  .  .  5 |  .  3  . | +1 +4  8 |
		/// :----------+----------+----------:
		/// |  8  .  4 |  . +5  . | +7  6  . |
		/// |  .  9  . |  .  2  . | +8  5 +4 |
		/// |  .  5  . |  .  .  . |  9  .  3 |
		/// :----------+----------+----------:
		/// |  5  .  . |  .  8  . |  6  . +9 |
		/// | +4  .  8 |  1  9  . |  5  .  . |
		/// |  .  .  9 |  .  . +5 |  4  8  . |
		/// '----------'----------'----------'
		/// </code>
		/// (Due to the rendering engine of IDE, all spaces will be displayed only once
		/// in this table.)
		/// </para>
		/// </summary>
		Table,

		/// <summary>
		/// <para>
		/// Sudoku explainer format (9 characters in a row, and 9 rows).
		/// </para>
		/// <para>
		/// For example:
		/// <code>
		/// 080630040
		/// 200085009
		/// 090000081
		/// 000300800
		/// 000020000
		/// 006001000
		/// 970000030
		/// 400850007
		/// 010094050
		/// </code>
		/// </para>
		/// </summary>
		SimpleTable,

		/// <summary>
		/// <para>
		/// Sukaku sudoku format (only contains the digits and the whitespace).
		/// </para>
		/// <para>
		/// For example:
		/// <code>
		///  23456789  23456789  23456789 123456789 123456789  23456789 123456789  23456789  23456789
		///  23456789 123456789 123456789  23456789  23456789  23456789  23456789  23456789  23456789
		///  23456789  23456789  23456789  23456789 123456789  23456789 123456789  23456789 123456789
		/// 123456789  23456789  23456789 123456789 123456789  23456789  23456789  23456789 123456789
		///  23456789  23456789  23456789  23456789 123456789  23456789 123456789  23456789  23456789
		/// 123456789  23456789  23456789 123456789 123456789  23456789 123456789  23456789 123456789
		///  23456789 123456789 123456789  23456789  23456789 123456789  23456789 123456789  23456789
		///  23456789 123456789 123456789  23456789  23456789 123456789  23456789 123456789  23456789
		///  23456789 123456789 123456789  23456789  23456789 123456789  23456789 123456789  23456789
		/// </code>
		/// (Due to the rendering engine of IDE, all spaces will be displayed only once
		/// in this table. In addition, this puzzle doesn't have a unique solution, here it is a
		/// model for references only.)
		/// </para>
		/// </summary>
		Sukaku,

		/// <summary>
		/// <para>Sukaku single line format, which uses '0' to be a placeholder.</para>
		/// <para>
		/// For example:
		/// 023406789123456700003050780120050789003006009123456009100056089023456080123000789003456789023006700023006700120056780100006789120400780000406789103000080020450000003406089020056709120050080100000700120056780120000709123056000123050080000450089123050709100456089003400780100450009003400009120400009123056709003000700023406000000406009123006000000056709003406700120050009103056089103456709003056009120456009123456780023400000103406709003056789020406700100050009020400000100006789020006789020456780123056080120400700000406789120400080023456789003006080020456789103450709123400009123400080023406700123406709103400080123456009120456789023406709023450080100406080103056009100400009123000789100400709100406780123050700000050080023406009
		/// </para>
		/// </summary>
		SukakuSingleLine,

		/// <summary>
		/// <para>
		/// Excel sudoku format (only contains the digits and the tab character).
		/// </para>
		/// <para>
		/// For example:
		/// <code>
		/// 1			7	8	9	4	5	6
		/// 4	5	6	1	2	3	7	8	9
		/// 7	8	9	4	5	6	1	2	
		/// 9	1	2	6		8	3	4	
		/// 3	4	5				6	7	8
		/// 	7	8	3		5	9	1	2
		/// 	9	1	5	6	7	2	3	4
		/// 2	3	4	8	9	1	5	6	7
		/// 5	6	7	2	3	4			1
		/// </code>
		/// (Due to the rendering engine of IDE, all spaces will be displayed only once
		/// in this table.)
		/// </para>
		/// </summary>
		Excel,
	}
}
