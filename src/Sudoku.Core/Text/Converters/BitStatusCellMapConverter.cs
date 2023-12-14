using Sudoku.Concepts;

namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a converter that uses bit operation to format <see cref="CellMap"/> instances.
/// </summary>
/// <seealso cref="CellMap"/>
public sealed record BitStatusCellMapConverter : ISpecifiedConceptConverter<CellMap>
{
	/// <inheritdoc/>
	public FuncRefReadOnly<CellMap, string> Converter
		=> static (scoped ref readonly CellMap cells) =>
		{
			scoped var result = (stackalloc char[81]);
			result.Fill('0');

			for (var cell = 0; cell < 81; cell++)
			{
				if (cells.Contains(cell))
				{
					result[cell] = '1';
				}
			}

			return result.ToString();
		};
}
