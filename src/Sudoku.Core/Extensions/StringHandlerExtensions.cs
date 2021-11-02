namespace System.Text;

/// <summary>
/// Provides extension methods on <see cref="StringHandler"/>.
/// </summary>
/// <seealso cref="StringHandler"/>
public static class StringHandlerExtensions
{
	/// <summary>
	/// Appends a sudoku grid into the handler.
	/// </summary>
	/// <param name="this">The handler.</param>
	/// <param name="grid">The sudoku grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AppendGrid(this ref StringHandler @this, in Grid grid) =>
		@this.AppendStringDirect(grid.ToString());

	/// <summary>
	/// Appends a sudoku grid into the handler.
	/// </summary>
	/// <param name="this">The handler.</param>
	/// <param name="grid">The sudoku grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete($"Please use type '{nameof(Grid)}' instead.", false)]
	public static void AppendGrid(this ref StringHandler @this, in SudokuGrid grid) =>
		@this.AppendStringDirect(grid.ToString());

	/// <summary>
	/// Append a sudoku grid into the handler.
	/// </summary>
	/// <param name="this">The handler.</param>
	/// <param name="grid">The sudoku grid.</param>
	/// <param name="alignment">Alignment value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AppendGridFormatted(this ref StringHandler @this, in Grid grid, int alignment) =>
		@this.AppendStringDirect(alignment switch
		{
			> 0 => grid.ToString().PadLeft(alignment),
			< 0 => grid.ToString().PadRight(alignment),
			_ => grid.ToString()
		});

	/// <summary>
	/// Append a sudoku grid into the handler.
	/// </summary>
	/// <param name="this">The handler.</param>
	/// <param name="grid">The sudoku grid.</param>
	/// <param name="format">The format string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AppendGridFormatted(this ref StringHandler @this, in Grid grid, string format) =>
		@this.AppendStringDirect(grid.ToString(format));

	/// <summary>
	/// Append a sudoku grid into the handler.
	/// </summary>
	/// <param name="this">The handler.</param>
	/// <param name="grid">The sudoku grid.</param>
	/// <param name="alignment">Alignment value.</param>
	/// <param name="format">The format string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void AppendGridFormatted(
		this ref StringHandler @this,
		in Grid grid,
		int alignment,
		string format
	) => @this.AppendStringDirect(alignment switch
	{
		> 0 => grid.ToString(format).PadLeft(alignment),
		< 0 => grid.ToString(format).PadRight(alignment),
		_ => grid.ToString(format)
	});

	/// <summary>
	/// Append a sudoku grid into the handler.
	/// </summary>
	/// <param name="this">The handler.</param>
	/// <param name="grid">The sudoku grid.</param>
	/// <param name="alignment">Alignment value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete($"Please use type '{nameof(Grid)}' instead.", false)]
	public static void AppendGridFormatted(this ref StringHandler @this, in SudokuGrid grid, int alignment) =>
		@this.AppendStringDirect(alignment switch
		{
			> 0 => grid.ToString().PadLeft(alignment),
			< 0 => grid.ToString().PadRight(alignment),
			_ => grid.ToString()
		});

	/// <summary>
	/// Append a sudoku grid into the handler.
	/// </summary>
	/// <param name="this">The handler.</param>
	/// <param name="grid">The sudoku grid.</param>
	/// <param name="format">The format string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete($"Please use type '{nameof(Grid)}' instead.", false)]
	public static void AppendGridFormatted(this ref StringHandler @this, in SudokuGrid grid, string format) =>
		@this.AppendStringDirect(grid.ToString(format));

	/// <summary>
	/// Append a sudoku grid into the handler.
	/// </summary>
	/// <param name="this">The handler.</param>
	/// <param name="grid">The sudoku grid.</param>
	/// <param name="alignment">Alignment value.</param>
	/// <param name="format">The format string.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[Obsolete($"Please use type '{nameof(Grid)}' instead.", false)]
	public static void AppendGridFormatted(
		this ref StringHandler @this,
		in SudokuGrid grid,
		int alignment,
		string format
	) => @this.AppendStringDirect(alignment switch
	{
		> 0 => grid.ToString(format).PadLeft(alignment),
		< 0 => grid.ToString(format).PadRight(alignment),
		_ => grid.ToString(format)
	});
}
