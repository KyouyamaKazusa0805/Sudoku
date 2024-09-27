namespace Sudoku.Concepts.Primitives;

/// <summary>
/// Represents a type that supports operations used by a sudoku grid.
/// </summary>
/// <typeparam name="TSelf"><include file="../../global-doc-comments.xml" path="/g/self-type-constraint"/></typeparam>
public interface IGridOperations<TSelf> : IGridConstants<TSelf> where TSelf : unmanaged, IGridOperations<TSelf>
{
	/// <summary>
	/// Gets the mask at the specified position.
	/// </summary>
	/// <param name="cell">The desired cell index.</param>
	/// <returns>The reference to the mask.</returns>
	[UnscopedRef]
	public abstract ref Mask this[Cell cell] { get; }

	/// <summary>
	/// Creates a mask of type <see cref="Mask"/> that represents the usages of digits 1 to 9,
	/// ranged in a specified list of cells in the current sudoku grid.
	/// </summary>
	/// <param name="cells">A list of desired cells.</param>
	/// <returns>A mask of type <see cref="Mask"/> that represents the usages of digits 1 to 9.</returns>
	public abstract Mask this[ref readonly CellMap cells] { get; }

	/// <summary>
	/// <inheritdoc cref="this[ref readonly CellMap]" path="/summary"/>
	/// </summary>
	/// <param name="cells"><inheritdoc cref="this[ref readonly CellMap]" path="/param[@name='cells']"/></param>
	/// <param name="withValueCells">
	/// Indicates whether the value cells (given or modifiable ones) will be included to be checked.
	/// If <see langword="true"/>, all value cells (no matter what kind of cell) will be summed up.
	/// </param>
	/// <param name="mergingMethod">
	/// Indicates the merging method. Values are <c>'<![CDATA[&]]>'</c>, <c>'<![CDATA[|]]>'</c> and <c>'<![CDATA[~]]>'</c>.
	/// <list type="bullet">
	/// <item><c>'<![CDATA[&]]>'</c>: Use <b>bitwise and</b> operator to merge masks.</item>
	/// <item><c>'<![CDATA[|]]>'</c>: Use <b>bitwise or</b> operator to merge masks.</item>
	/// <item><c>'<![CDATA[~]]>'</c>: Use <b>bitwise nand</b> operator to merge masks.</item>
	/// </list>
	/// By default, the value is <c>'<![CDATA[|]]>'</c>.
	/// </param>
	/// <returns><inheritdoc cref="this[ref readonly CellMap]" path="/returns"/></returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when <paramref name="mergingMethod"/> is not defined.</exception>
	public abstract Mask this[ref readonly CellMap cells, bool withValueCells, [ConstantExpected] char mergingMethod = '|'] { get; }


	/// <summary>
	/// Reset the sudoku grid, making all modifiable values to empty ones.
	/// </summary>
	public abstract void Reset();

	/// <summary>
	/// To fix the current grid (all modifiable values will be changed to given ones).
	/// </summary>
	public abstract void Fix();

	/// <summary>
	/// To unfix the current grid (all given values will be changed to modifiable ones).
	/// </summary>
	public abstract void Unfix();

	/// <summary>
	/// Try to apply the specified conclusion.
	/// </summary>
	/// <param name="conclusion">The conclusion to be applied.</param>
	public abstract void Apply(Conclusion conclusion);

	/// <summary>
	/// Set the specified cell to the specified state.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="state">The state.</param>
	public abstract void SetState(Cell cell, CellState state);

	/// <summary>
	/// Set the specified cell with specified candidates.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask that holds a list of desired digits.</param>
	public abstract void SetCandidates(Cell cell, Mask mask);

	/// <summary>
	/// Set the specified cell to the specified mask.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="mask">The mask to set.</param>
	public abstract void SetMask(Cell cell, Mask mask);

	/// <summary>
	/// Set the specified digit into the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">
	/// <para>
	/// The value you want to set. The value should be between 0 and 8.
	/// If assigning -1, the grid will execute an implicit behavior that candidates in <b>all</b> empty cells will be re-computed.
	/// </para>
	/// <para>
	/// The values set into the grid will be regarded as the modifiable values.
	/// If the cell contains a digit, it will be covered when it is a modifiable value.
	/// If the cell is a given cell, the setter will do nothing.
	/// </para>
	/// </param>
	public abstract void SetDigit(Cell cell, Digit digit);

	/// <summary>
	/// Sets the target candidate state.
	/// </summary>
	/// <param name="cell">The cell offset between 0 and 80.</param>
	/// <param name="digit">The digit between 0 and 8.</param>
	/// <param name="isOn">
	/// The case you want to set. <see langword="false"/> means that this candidate
	/// doesn't exist in this current sudoku grid; otherwise, <see langword="true"/>.
	/// </param>
	public abstract void SetExistence(Cell cell, Digit digit, bool isOn);

	/// <summary>
	/// Sets a candidate existence case with a <see cref="bool"/> value.
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating that.</returns>
	/// <inheritdoc cref="SetExistence(Cell, Digit, bool)"/>
	public abstract bool GetExistence(Cell cell, Digit digit);

	/// <inheritdoc cref="Exists(Cell, Digit)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public virtual bool? Exists(Candidate candidate) => Exists(candidate / CellCandidatesCount, candidate % CellCandidatesCount);

	/// <summary>
	/// Indicates whether the current grid contains the digit in the specified cell.
	/// </summary>
	/// <param name="cell">The cell to be checked.</param>
	/// <param name="digit">The digit to be checked.</param>
	/// <returns>
	/// The method will return a <see cref="bool"/>? value
	/// (containing three possible cases: <see langword="true"/>, <see langword="false"/> and <see langword="null"/>).
	/// All values corresponding to the cases are below:
	/// <list type="table">
	/// <listheader>
	/// <term>Value</term>
	/// <description>Case description on this value</description>
	/// </listheader>
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
	/// <para>
	/// Note that the method will return a <see cref="bool"/>?, so you should use the code
	/// '<c>grid.Exists(cell, digit) is true</c>' or '<c>grid.Exists(cell, digit) == true</c>'
	/// to decide whether a condition is true.
	/// </para>
	/// <para>
	/// In addition, because the type is <see cref="bool"/>? rather than <see cref="bool"/>,
	/// the result case will be more precisely than the indexer <see cref="GetExistence(Cell, Digit)"/>,
	/// which is the main difference between this method and that indexer.
	/// </para>
	/// </remarks>
	/// <seealso cref="GetExistence(Cell, Digit)"/>
	public abstract bool? Exists(Cell cell, Digit digit);

	/// <summary>
	/// Get the cell state at the specified cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <returns>The cell state.</returns>
	public abstract CellState GetState(Cell cell);

	/// <summary>
	/// Get the candidate mask part of the specified cell.
	/// </summary>
	/// <param name="cell">The cell offset you want to get.</param>
	/// <returns>
	/// <para>
	/// The candidate mask. The return value is a 9-bit <see cref="Mask"/> value, where each bit will be:
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
	public abstract Mask GetCandidates(Cell cell);

	/// <summary>
	/// Try to get the digit filled in the specified cell.
	/// </summary>
	/// <param name="cell">The cell used.</param>
	/// <returns>The digit that the current cell filled. If the cell is empty, return -1.</returns>
	/// <exception cref="InvalidOperationException">Throws when the specified cell keeps a wrong cell state value.</exception>
	public abstract Digit GetDigit(Cell cell);
}
