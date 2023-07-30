namespace Sudoku.Text.Formatting;

/// <summary>
/// Defines a formatter that formats a <see cref="CellMap"/>, converting into a table <see cref="string"/> value to display all values.
/// </summary>
public sealed record CellMapTableFormat : ICellMapFormatter
{
	/// <inheritdoc/>
	public string ToString(scoped in CellMap cellMap)
	{
		var newLine = Environment.NewLine;
		scoped var sb = new StringHandler((3 * 7 + newLine.Length) * 13 - newLine.Length);
		for (var i = 0; i < 3; i++)
		{
			for (var bandLn = 0; bandLn < 3; bandLn++)
			{
				for (var j = 0; j < 3; j++)
				{
					for (var columnLn = 0; columnLn < 3; columnLn++)
					{
						sb.Append(cellMap.Contains((i * 3 + bandLn) * 9 + j * 3 + columnLn) ? '*' : '.');
						sb.Append(' ');
					}

					if (j != 2)
					{
						sb.Append("| ");
					}
					else
					{
						sb.AppendLine();
					}
				}
			}

			if (i != 2)
			{
				sb.Append("------+-------+------");
				sb.AppendLine();
			}
		}

		sb.RemoveFromEnd(newLine.Length);
		return sb.ToStringAndClear();
	}
}
