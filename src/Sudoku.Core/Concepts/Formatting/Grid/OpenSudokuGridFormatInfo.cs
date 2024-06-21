namespace Sudoku.Concepts;

/// <summary>
/// Represents a <see cref="GridFormatInfo"/> type that supports OpenSudoku formatting.
/// </summary>
public sealed partial class OpenSudokuGridFormatInfo : GridFormatInfo
{
	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(GridFormatInfo) ? this : null;

	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] GridFormatInfo? other) => other is OpenSudokuGridFormatInfo;

	/// <inheritdoc/>
	public override int GetHashCode() => HashCode.Combine(typeof(OpenSudokuGridFormatInfo));

	/// <inheritdoc/>
	public override OpenSudokuGridFormatInfo Clone() => new();

	/// <inheritdoc/>
	protected internal override string FormatGrid(ref readonly Grid grid)
	{
		// Calculates the length of the result string.
		const int length = 1 + (81 * 3 - 1 << 1);

		// Creates a string instance as a buffer.
		var result = new string('\0', length);

		// Modify the string value via pointers.
		ref var pResult = ref result.MutableRef();

		// Replace the base character with the separator.
		for (var pos = 1; pos < length; pos += 2)
		{
			@ref.Add(ref pResult, pos) = '|';
		}

		// Now replace some positions with the specified values.
		for (var (i, pos) = (0, 0); i < 81; i++, pos += 6)
		{
			switch (grid.GetState(i))
			{
				case CellState.Empty:
				{
					@ref.Add(ref pResult, pos) = '0';
					@ref.Add(ref pResult, pos + 2) = '0';
					@ref.Add(ref pResult, pos + 4) = '1';
					break;
				}
				case CellState.Modifiable:
				case CellState.Given:
				{
					@ref.Add(ref pResult, pos) = (char)(grid.GetDigit(i) + '1');
					@ref.Add(ref pResult, pos + 2) = '0';
					@ref.Add(ref pResult, pos + 4) = '0';
					break;
				}
				default:
				{
					throw new FormatException(SR.ExceptionMessage("GridInvalid"));
				}
			}
		}

		// Returns the result.
		return result;
	}

	/// <inheritdoc/>
	protected internal override Grid ParseGrid(string str)
	{
		if (GridOpenSudokuPattern().Match(str) is not { Success: true, Value: var match })
		{
			return Grid.Undefined;
		}

		var result = Grid.Empty;
		for (var i = 0; i < 81; i++)
		{
			switch (match[i * 6])
			{
				case '0' when whenClause(i * 6, match, "|0|1", "|0|1|"):
				{
					continue;
				}
				case not '0' and var ch when whenClause(i * 6, match, "|0|0", "|0|0|"):
				{
					result.SetDigit(i, ch - '1');
					result.SetState(i, CellState.Given);

					break;
				}
				default:
				{
					// Invalid string state.
					return Grid.Undefined;
				}
			}
		}

		return result;


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static bool whenClause(Cell i, string match, string pattern1, string pattern2)
			=> i == 80 * 6 ? match[(i + 1)..(i + 5)] == pattern1 : match[(i + 1)..(i + 6)] == pattern2;
	}


	[GeneratedRegex("""\d(\|\d){242}""", RegexOptions.Compiled, 5000)]
	public static partial Regex GridOpenSudokuPattern();
}
