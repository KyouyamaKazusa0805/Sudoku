extern alias extended;

namespace Sudoku.Data;

/// <summary>
/// Defines a data structure that describes a sudoku grid.
/// </summary>
/// <typeparam name="TGrid">The type to implement this interface.</typeparam>
internal unsafe interface IGrid<TGrid> : IValueEquatable<TGrid>, IFormattable, extended::System.IParseable<TGrid>
where TGrid : struct, IGrid<TGrid>
{
	/// <summary>
	/// Indicates the default mask of a cell (an empty cell, with all 9 candidates left).
	/// </summary>
	static abstract short DefaultMask { get; }

	/// <summary>
	/// Indicates the maximum candidate mask that used.
	/// </summary>
	static abstract short MaxCandidatesMask { get; }

	/// <summary>
	/// Indicates the empty mask, modifiable mask and given mask.
	/// </summary>
	static abstract short EmptyMask { get; }

	/// <summary>
	/// Indicates the modifiable mask.
	/// </summary>
	static abstract short ModifiableMask { get; }

	/// <summary>
	/// Indicates the given mask.
	/// </summary>
	static abstract short GivenMask { get; }

	/// <summary>
	/// Indicates the empty grid string.
	/// </summary>
	static abstract string EmptyString { get; }

	/// <summary>
	/// Indicates the event triggered when the value is changed.
	/// </summary>
	static abstract void* ValueChanged { get; }

	/// <summary>
	/// Indicates the event triggered when should re-compute candidates.
	/// </summary>
	static abstract void* RefreshingCandidates { get; }

	/// <summary>
	/// Indicates the default grid that all values are initialized 0, which is same as
	/// <typeparamref name="TGrid"/>().
	/// </summary>
	/// <remarks>
	/// We recommend you should use this static field instead of the default constructor
	/// to reduce object creation.
	/// </remarks>
	static abstract TGrid Undefined { get; }

	/// <summary>
	/// The empty grid that is valid during implementation or running the program
	/// (all values are <see cref="DefaultMask"/>, i.e. empty cells).
	/// </summary>
	/// <remarks>
	/// This field is initialized by the static constructor of this structure.
	/// </remarks>
	/// <seealso cref="DefaultMask"/>
	static abstract TGrid Empty { get; }

	/// <summary>
	/// Indicates the grid has already solved. If the value is <see langword="true"/>,
	/// the grid is solved; otherwise, <see langword="false"/>.
	/// </summary>
	bool IsSolved { get; }

	/// <summary>
	/// Indicates whether the grid is <see cref="Undefined"/>, which means the grid
	/// holds totally same value with <see cref="Undefined"/>.
	/// </summary>
	/// <seealso cref="Undefined"/>
	bool IsUndefined { get; }

	/// <summary>
	/// Indicates whether the grid is <see cref="Empty"/>, which means the grid
	/// holds totally same value with <see cref="Empty"/>.
	/// </summary>
	/// <seealso cref="Empty"/>
	bool IsEmpty { get; }

#if DEBUG
	/// <summary>
	/// Indicates whether the grid is as same behaviors as <see cref="Undefined"/>
	/// in debugging mode.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This property checks whether all non-first masks are all 0. This checking behavior
	/// is aiming to the debugger because the debugger can't recognize the fixed buffer.
	/// </para>
	/// <para>
	/// The debugger can't recognize fixed buffer.
	/// The fixed buffer whose code is like:
	/// <code><![CDATA[
	/// private fixed short _values[81];
	/// ]]></code>
	/// However, internally, the field <c>_values</c> is implemented
	/// with a fixed buffer using a inner struct, which is just like:
	/// <code><![CDATA[
	/// [StructLayout(LayoutKind.Explicit, Size = 81 * sizeof(short))]
	/// private struct FixedBuffer
	/// {
	///     public short _internalValue;
	/// }
	/// ]]></code>
	/// And that field:
	/// <code><![CDATA[
	/// private FixedBuffer _fixedField;
	/// ]]></code>
	/// From the code we can learn that only 2 bytes of the inner struct can be detected,
	/// because the buffer struct only contains 2 bytes data.
	/// </para>
	/// </remarks>
	/// <see cref="Undefined"/>
	bool IsDebuggerUndefined { get; }
#endif

	/// <summary>
	/// Indicates the number of total candidates.
	/// </summary>
	int CandidatesCount { get; }

	/// <summary>
	/// Indicates the total number of given cells.
	/// </summary>
	int GivensCount { get; }

	/// <summary>
	/// Indicates the total number of modifiable cells.
	/// </summary>
	int ModifiablesCount { get; }

	/// <summary>
	/// Indicates the total number of empty cells.
	/// </summary>
	int EmptiesCount { get; }

	/// <summary>
	/// Gets the token of this sudoku grid.
	/// </summary>
	string Token { get; }

	/// <summary>
	/// Indicates the eigen string value that can introduce the current sudoku grid.
	/// </summary>
	string EigenString { get; }

	/// <summary>
	/// Indicates the cells that corresponding position in this grid is empty.
	/// </summary>
	Cells EmptyCells { get; }

	/// <summary>
	/// Indicates the cells that corresponding position in this grid contain two candidates.
	/// </summary>
	Cells BivalueCells { get; }

	/// <summary>
	/// Indicates the map of possible positions of the existence of the candidate value for each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </summary>
	Cells[] CandidatesMap { get; }

	/// <summary>
	/// <para>
	/// Indicates the map of possible positions of the existence of each digit. The return value will
	/// be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </para>
	/// <para>
	/// Different with <see cref="CandidatesMap"/>, this property contains all givens, modifiables and
	/// empty cells only if it contains the digit in the mask.
	/// </para>
	/// </summary>
	/// <seealso cref="CandidatesMap"/>
	Cells[] DigitsMap { get; }

	/// <summary>
	/// <para>
	/// Indicates the map of possible positions of the existence of that value of each digit.
	/// The return value will be an array of 9 elements, which stands for the statuses of 9 digits.
	/// </para>
	/// <para>
	/// Different with <see cref="CandidatesMap"/>, the value only contains the given or modifiable
	/// cells whose mask contain the set bit of that digit.
	/// </para>
	/// </summary>
	/// <seealso cref="CandidatesMap"/>
	Cells[] ValuesMap { get; }


	/// <summary>
	/// Gets or sets the value in the specified cell.
	/// </summary>
	/// <param name="cell">The cell you want to get or set a value.</param>
	/// <value>
	/// The value you want to set. The value should be between 0 and 8. If assigning -1,
	/// that means to re-compute all candidates.
	/// </value>
	/// <returns>
	/// The value that the cell filled with. The possible values are:
	/// <list type="table">
	/// <item>
	/// <term>-2</term>
	/// <description>The status of the specified cell is <see cref="CellStatus.Undefined"/>.</description>
	/// </item>
	/// <item>
	/// <term>-1</term>
	/// <description>The status of the specified cell is <see cref="CellStatus.Empty"/>.</description>
	/// </item>
	/// <item>
	/// <term>0 to 8</term>
	/// <description>
	/// The actual value that the cell filled with. 0 is for the digit 1, 1 is for the digit 2, etc..
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	int this[int cell] { get; set; }

	/// <summary>
	/// Gets or sets a candidate existence case with a <see cref="bool"/> value.
	/// </summary>
	/// <param name="cell">The cell offset between 0 and 80.</param>
	/// <param name="digit">The digit between 0 and 8.</param>
	/// <value>
	/// The case you want to set. <see langword="false"/> means that this candidate
	/// doesn't exist in this current sudoku grid; otherwise, <see langword="true"/>.
	/// </value>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	bool this[int cell, int digit] { get; set; }


	/// <summary>
	/// Check whether the current grid is valid (no duplicate values on same row, column or block).
	/// </summary>
	/// <returns>The <see cref="bool"/> result.</returns>
	bool SimplyValidate();

	/// <summary>
	/// Indicates whether the current grid contains the specified candidate offset.
	/// </summary>
	/// <param name="candidate">The candidate offset.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value (contains three possible cases:
	/// <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>
	/// The cell that the candidate specified is an empty cell <b>and</b> contains the specified digit
	/// that the candidate specified.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>
	/// The cell that the candidate specified is an empty cell <b>but doesn't</b> contain the specified digit
	/// that the candidate specified.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>
	/// The cell that the candidate specified is <b>not</b> an empty cell that the candidate specified.
	/// </description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// Note that the method will return a <see cref="bool"/>?, so you should use the code
	/// '<c>grid.Exists(candidate) is true</c>' or '<c>grid.Exists(candidate) == true</c>'
	/// to decide whether a condition is true.
	/// </remarks>
	bool? Exists(int candidate);

	/// <summary>
	/// Indicates whether the current grid contains the digit in the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value (contains three possible cases:
	/// <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
	/// <item>
	/// <term><see langword="true"/></term>
	/// <description>
	/// The cell is an empty cell <b>and</b> contains the specified digit.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="false"/></term>
	/// <description>
	/// The cell is an empty cell <b>but doesn't</b> contain the specified digit.
	/// </description>
	/// </item>
	/// <item>
	/// <term><see langword="null"/></term>
	/// <description>The cell is <b>not</b> an empty cell.</description>
	/// </item>
	/// </list>
	/// </returns>
	/// <remarks>
	/// Note that the method will return a <see cref="bool"/>?, so you should use the code
	/// '<c>grid.Exists(cell, digit) is true</c>' or '<c>grid.Exists(cell, digit) == true</c>'
	/// to decide whether a condition is true.
	/// </remarks>
	bool? Exists(int cell, int digit);

	/// <inheritdoc cref="object.GetHashCode"/>
	int GetHashCode();

	/// <summary>
	/// Serializes this instance to an array, where all digit value will be stored.
	/// </summary>
	/// <returns>
	/// This array. All elements are between 0 to 9, where 0 means the
	/// cell is <see cref="CellStatus.Empty"/> now.
	/// </returns>
	int[] ToArray();

	/// <summary>
	/// Get a mask at the specified cell.
	/// </summary>
	/// <param name="offset">The cell offset you want to get.</param>
	/// <returns>The mask.</returns>
	short GetMask(int offset);

	/// <summary>
	/// Get the candidate mask part of the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset you want to get.</param>
	/// <returns>
	/// <para>
	/// The candidate mask. The return value is a 9-bit <see cref="short"/>
	/// value, where each bit will be:
	/// <list type="table">
	/// <item>
	/// <term><c>0</c></term>
	/// <description>The cell <b>doesn't contain</b> the possibility of the digit.</description>
	/// </item>
	/// <item>
	/// <term><c>1</c></term>
	/// <description>The cell <b>contains</b> the possibility of the digit.</description>
	/// </item>
	/// </list>
	/// </para>
	/// <para>
	/// For example, if the result mask is 266 (i.e. <c>0b<b>1</b>00_00<b>1</b>_0<b>1</b>0</c> in binary),
	/// the value will indicate the cell contains the digit 2, 4 and 9.
	/// </para>
	/// </returns>
	short GetCandidates(int cell);

	/// <summary>
	/// Returns a reference to the element of the <typeparamref name="TGrid"/> at index zero.
	/// </summary>
	/// <returns>A reference to the element of the <typeparamref name="TGrid"/> at index zero.</returns>
	ref readonly short GetPinnableReference();

	/// <summary>
	/// Get all masks and print them.
	/// </summary>
	/// <returns>The result.</returns>
	/// <remarks>
	/// Please note that the method cannot be called with a correct behavior using
	/// <see cref="DebuggerDisplayAttribute"/> to output. It seems that Visual Studio
	/// doesn't print correct values when indices of this grid aren't 0. In other words,
	/// when we call this method using <see cref="DebuggerDisplayAttribute"/>, only <c>grid[0]</c>
	/// can be output correctly, and other values will be incorrect: they're always 0.
	/// </remarks>
	string ToMaskString();

	/// <summary>
	/// Get the cell status at the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The cell status.</returns>
	CellStatus GetStatus(int cell);

	/// <summary>
	/// To fix the current grid (all modifiable values will be changed to given ones).
	/// </summary>
	void Fix();

	/// <summary>
	/// To unfix the current grid (all given values will be changed to modifiable ones).
	/// </summary>
	void Unfix();

	/// <summary>
	/// Set the specified cell to the specified status.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="status">The status.</param>
	void SetStatus(int cell, CellStatus status);

	/// <summary>
	/// Set the specified cell to the specified mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask to set.</param>
	void SetMask(int cell, short mask);

	/// <summary>
	/// To determine whether two sudoku grid is totally same.
	/// </summary>
	/// <param name="left">The left one.</param>
	/// <param name="right">The right one.</param>
	/// <returns>The <see cref="bool"/> result indicating that.</returns>
	static abstract bool Equals(in TGrid left, in TGrid right);

	/// <summary>
	/// <para>Parses a string value and converts to this type.</para>
	/// <para>
	/// If you want to parse a PM grid, we recommend you use the method
	/// <see cref="Parse(string, GridParsingOption)"/> instead of this method.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <returns>The result instance had converted.</returns>
	/// <seealso cref="Parse(string, GridParsingOption)"/>
	static abstract TGrid Parse(in ReadOnlySpan<char> str);

	/// <summary>
	/// Parses a pointer that points to a string value and converts to this type.
	/// </summary>
	/// <param name="ptrStr">The pointer that points to string.</param>
	/// <returns>The result instance.</returns>
	static abstract TGrid Parse(char* ptrStr);

	/// <summary>
	/// <para>
	/// Parses a string value and converts to this type.
	/// </para>
	/// <para>
	/// If you want to parse a PM grid, you should decide the mode to parse.
	/// If you use compatible mode to parse, all single values will be treated as
	/// given values; otherwise, recommended mode, which uses '<c><![CDATA[<d>]]></c>'
	/// or '<c>*d*</c>' to represent a value be a given or modifiable one. The decision
	/// will be indicated and passed by the second parameter <paramref name="compatibleFirst"/>.
	/// </para>
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="compatibleFirst">
	/// Indicates whether the parsing operation should use compatible mode to check PM grid.
	/// </param>
	/// <returns>The result instance had converted.</returns>
	static abstract TGrid Parse(string? str, bool compatibleFirst);

	/// <summary>
	/// Parses a string value and converts to this type, using a specified grid parsing type.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="gridParsingOption">The grid parsing type.</param>
	/// <returns>The result instance had converted.</returns>
	static abstract TGrid Parse(string? str, GridParsingOption gridParsingOption);

	/// <summary>
	/// Try to parse a string and converts to this type, and returns a
	/// <see cref="bool"/> value indicating the result of the conversion.
	/// </summary>
	/// <param name="str">The string.</param>
	/// <param name="option">The grid parsing type.</param>
	/// <param name="result">
	/// The result parsed. If the conversion is failed,
	/// this argument will be <see cref="Undefined"/>.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <seealso cref="Undefined"/>
	static abstract bool TryParse([NotNullWhen(true)] string? str, GridParsingOption option, [DiscardWhen(false)] out TGrid result);
}
