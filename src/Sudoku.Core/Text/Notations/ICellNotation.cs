namespace Sudoku.Text.Notations;

/// <summary>
/// Defines a type that can convert a <see cref="CellMap"/> instance into a result <see cref="string"/>
/// representation to describe the cell collection.
/// </summary>
/// <typeparam name="TSelf">The base type that applies the interface.</typeparam>
/// <typeparam name="TOptions">The type that is used as the provider for extra options.</typeparam>
public interface ICellNotation<[Self] TSelf, TOptions>
	where TSelf : class, ICellNotation<TSelf, TOptions>
	where TOptions : struct, INotationOptions<TOptions>
{
	/// <summary>
	/// Indicates the cell notation kind that the current type supports.
	/// </summary>
	public static abstract CellNotation CellNotation { get; }


	/// <summary>
	/// <para>
	/// Try to parse the specified <see cref="string"/> value, and convert it into the <see cref="CellMap"/>
	/// instance.
	/// </para>
	/// <para>
	/// Different with the method <see cref="ParseCells(string)"/>, the method will return a
	/// <see cref="bool"/> value instead, indicating whether the operation is successful. Therefore,
	/// the method won't throw <see cref="FormatException"/>.
	/// </para>
	/// </summary>
	/// <param name="str">The <see cref="string"/> value.</param>
	/// <param name="result">
	/// The <see cref="CellMap"/> result. If the return value is <see langword="false"/>,
	/// this argument will be a discard and cannot be used.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether the parsing operation is successful.</returns>
	/// <seealso cref="ParseCells(string)"/>
	public static abstract bool TryParseCells(string str, out CellMap result);

	/// <summary>
	/// Gets the <see cref="string"/> representation of a list of cells.
	/// </summary>
	/// <param name="cells">The cell list.</param>
	/// <returns>The <see cref="string"/> representation describe the cell list.</returns>
	public static abstract string ToCellsString(scoped in CellMap cells);

	/// <summary>
	/// Gets the <see cref="string"/> representation of a list of cells.
	/// </summary>
	/// <param name="cells">The cell list.</param>
	/// <param name="options">The extra options to control the output style.</param>
	/// <returns>The <see cref="string"/> representation describe the cell list.</returns>
	public static abstract string ToCellsString(scoped in CellMap cells, scoped in TOptions options);

	/// <summary>
	/// Try to parse the specified <see cref="string"/> value, and convert it into the <see cref="CellMap"/>
	/// instance.
	/// </summary>
	/// <param name="str">The <see cref="string"/> value.</param>
	/// <returns>The <see cref="CellMap"/> result.</returns>
	/// <exception cref="FormatException">
	/// Throws when the parsing operation is failed due to invalid characters or invalid operation.
	/// </exception>
	public static abstract CellMap ParseCells(string str);
}
