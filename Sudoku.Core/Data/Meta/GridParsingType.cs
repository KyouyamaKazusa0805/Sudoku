namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Represents a grid parsing type.
	/// </summary>
	public enum GridParsingType : byte
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
		/// 080630040200085009090000081000300800000020000006001000970000030400850007010094050<br/>
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
		/// .--------------.--------------.-------------.<br/>
		/// | &lt;4&gt;   *8*    59  | 157  15   &lt;2&gt;  | *3*   79  &lt;6&gt;   |<br/>
		/// | *3*   &lt;1&gt;    256 | 578  568  *9*  | 25  78  &lt;4&gt;   |<br/>
		/// | 56  79   279 | *4*    568  &lt;3&gt;  | 29  &lt;1&gt;   58  |<br/>
		/// :--------------+--------------+-------------:<br/>
		/// | &lt;9&gt;   *3*    &lt;8&gt;   | *2*    &lt;4&gt;    *6*  | &lt;7&gt;   &lt;5&gt;   *1*   |<br/>
		/// | *7*   &lt;5&gt;    *1*   | *3*    &lt;9&gt;    *8*  | *4*   &lt;6&gt;   *2*   |<br/>
		/// | *2*   &lt;6&gt;    &lt;4&gt;   | 15   &lt;7&gt;    15 | &lt;8&gt;   *3*   &lt;9&gt;   |<br/>
		/// :--------------+--------------+-------------:<br/>
		/// | 56  &lt;2&gt;    567 | &lt;9&gt;    138  47 | 16  48  38  |<br/>
		/// | &lt;1&gt;   479  679 | 58   358  47 | 69  &lt;2&gt;   358 |<br/>
		/// | &lt;8&gt;   49   *3*   | &lt;6&gt;    *2*    15 | 15  49  &lt;7&gt;   |<br/>
		/// '--------------'--------------'-------------'
		/// </code>
		/// Due to the rendering engine of IDE, all spaces will not be displayed
		/// in this table.
		/// </para>
		/// </summary>
		PencilMarked,
		/// <summary>
		/// <para>
		/// Indicates the pencil marked grid (PM grid), which means all
		/// grid candidates will be displayed using a table. In addition,
		/// all single digit will be treated as a given digit.
		/// </para>
		/// <para>
		/// For example:
		/// <code>
		/// .--------------.--------------.-------------.<br/>
		/// | 4   8    59  | 157  15   2  | 3   79  6   |<br/>
		/// | 3   1    256 | 578  568  9  | 25  78  4   |<br/>
		/// | 56  79   279 | 4    568  3  | 29  1   58  |<br/>
		/// :--------------+--------------+-------------:<br/>
		/// | 9   3    8   | 2    4    6  | 7   5   1   |<br/>
		/// | 7   5    1   | 3    9    8  | 4   6   2   |<br/>
		/// | 2   6    4   | 15   7    15 | 8   3   9   |<br/>
		/// :--------------+--------------+-------------:<br/>
		/// | 56  2    567 | 9    138  47 | 16  48  38  |<br/>
		/// | 1   479  679 | 58   358  47 | 69  2   358 |<br/>
		/// | 8   49   3   | 6    2    15 | 15  49  7   |<br/>
		/// '--------------'--------------'-------------'
		/// </code>
		/// Due to the rendering engine of IDE, all spaces will not be displayed
		/// in this table.
		/// </para>
		/// </summary>
		PencilMarkedTreatSingleAsGiven,
		/// <summary>
		/// <para>
		/// Indicates the table format, which means all grid values
		/// will be displayed using a table with empty cell character
		/// <c>'0'</c> or <c>'.'</c>.
		/// </para>
		/// <para>
		/// For example:
		/// <code>
		/// .----------.----------.----------.<br/>
		/// |  .  4  6 |  . +1  . |  3 +7 +5 |<br/>
		/// | +3 +8  1 | +5  4  7 | +2 +9 +6 |<br/>
		/// |  .  .  5 |  .  3  . | +1 +4  8 |<br/>
		/// :----------+----------+----------:<br/>
		/// |  8  .  4 |  . +5  . | +7  6  . |<br/>
		/// |  .  9  . |  .  2  . | +8  5 +4 |<br/>
		/// |  .  5  . |  .  .  . |  9  .  3 |<br/>
		/// :----------+----------+----------:<br/>
		/// |  5  .  . |  .  8  . |  6  . +9 |<br/>
		/// | +4  .  8 |  1  9  . |  5  .  . |<br/>
		/// |  .  .  9 |  .  . +5 |  4  8  . |<br/>
		/// '----------'----------'----------'
		/// </code>
		/// Due to the rendering engine of IDE, all spaces will not be displayed
		/// in this table.
		/// </para>
		/// </summary>
		Table,
	}
}
