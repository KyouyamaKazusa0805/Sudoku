using Sudoku.Concepts;

namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a converter type for Open-Sudoku app format.
/// </summary>
public sealed record OpenSudokuGridConverter : ISpecifiedConceptConverter<Grid>
{
	/// <summary>
	/// Indicates the string terminator character.
	/// </summary>
	private const char Terminator = '\0';

	/// <summary>
	/// Indicates the separator character.
	/// </summary>
	private const char Separator = '|';

	/// <summary>
	/// Indicates the zero character.
	/// </summary>
	private const char Zero = '0';

	/// <summary>
	/// Indicates the one character.
	/// </summary>
	private const char One = '1';


	/// <inheritdoc/>
	public unsafe FuncRefReadOnly<Grid, string> Converter
		=> (scoped ref readonly Grid grid) =>
		{
			// Calculates the length of the result string.
			const int length = 1 + (81 * 3 - 1 << 1);

			// Creates a string instance as a buffer.
			var result = new string(Terminator, length);

			// Modify the string value via pointers.
			fixed (char* pResult = result)
			{
				// Replace the base character with the separator.
				for (var pos = 1; pos < length; pos += 2)
				{
					pResult[pos] = Separator;
				}

				// Now replace some positions with the specified values.
				for (var (i, pos) = (0, 0); i < 81; i++, pos += 6)
				{
					switch (grid.GetState(i))
					{
						case CellState.Empty:
						{
							pResult[pos] = Zero;
							pResult[pos + 2] = Zero;
							pResult[pos + 4] = One;
							break;
						}
						case CellState.Modifiable:
						case CellState.Given:
						{
							pResult[pos] = (char)(grid.GetDigit(i) + One);
							pResult[pos + 2] = Zero;
							pResult[pos + 4] = Zero;
							break;
						}
						default:
						{
							throw new FormatException("The specified grid is invalid.");
						}
					}
				}
			}

			// Returns the result.
			return result;
		};
}
