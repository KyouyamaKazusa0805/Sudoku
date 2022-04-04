namespace Sudoku.Concepts.Notations;

/// <summary>
/// Defines a type that can convert a <see cref="Cells"/> instance into a result <see cref="string"/>
/// representation to describe the cell collection.
/// </summary>
/// <typeparam name="TBaseType">The base type that applies the interface.</typeparam>
/// <typeparam name="TOptions">The type that is used as the provider for extra options.</typeparam>
public interface ICellNotation</*[Self]*/ TBaseType, TOptions>
	where TBaseType : class, INotationHandler, ICellNotation<TBaseType, TOptions>
	where TOptions : struct, INotationHandlerOptions<TOptions>, IDefaultable<TOptions>
{
	/// <summary>
	/// <para>
	/// Try to parse the specified <see cref="string"/> value, and convert it into the <see cref="Cells"/>
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
	/// The <see cref="Cells"/> result. If the return value is <see langword="false"/>,
	/// this argument will be a discard and cannot be used.
	/// </param>
	/// <returns>A <see cref="bool"/> value indicating whether the parsing operation is successful.</returns>
	/// <seealso cref="ParseCells(string)"/>
	static abstract bool TryParseCells(string str, out Cells result);

	/// <summary>
	/// Gets the <see cref="string"/> representation of a list of cells.
	/// </summary>
	/// <param name="cells">The cell list.</param>
	/// <returns>The <see cref="string"/> representation describe the cell list.</returns>
	static abstract string ToCellsString(in Cells cells);

	/// <summary>
	/// Gets the <see cref="string"/> representation of a list of cells.
	/// </summary>
	/// <param name="cells">The cell list.</param>
	/// <param name="options">The extra options to control the output style.</param>
	/// <returns>The <see cref="string"/> representation describe the cell list.</returns>
	static abstract string ToCellsString(in Cells cells, in TOptions options);

	/// <summary>
	/// Try to parse the specified <see cref="string"/> value, and convert it into the <see cref="Cells"/>
	/// instance.
	/// </summary>
	/// <param name="str">The <see cref="string"/> value.</param>
	/// <returns>The <see cref="Cells"/> result.</returns>
	/// <exception cref="FormatException">
	/// Throws when the parsing operation is failed due to invalid characters or invalid operation.
	/// </exception>
	static abstract Cells ParseCells(string str);
}
