using System.Text;
using Sudoku.Concepts;

namespace Sudoku.Text.Formatting;

/// <summary>
/// Represents with Excel formatter.
/// </summary>
public sealed record ExcelFormat : IGridFormatter
{
	/// <summary>
	/// Indicates the tab character.
	/// </summary>
	private const char Tab = '\t';

	/// <summary>
	/// Indicates the zero character.
	/// </summary>
	private const char Zero = '0';


	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static readonly ExcelFormat Default = new();


	/// <inheritdoc/>
	static IGridFormatter IGridFormatter.Instance => Default;


	/// <inheritdoc/>
	public string ToString(scoped ref readonly Grid grid)
	{
		scoped var span = grid.ToString(SusserFormat.Default with { Placeholder = Zero }).AsSpan();
		scoped var sb = new StringHandler(81 + 72 + 9);
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 9; j++)
			{
				if (span[i * 9 + j] - Zero is var digit and not 0)
				{
					sb.Append(digit);
				}

				sb.Append(Tab);
			}

			sb.RemoveFromEnd(1);
			sb.AppendLine();
		}

		sb.RemoveFromEnd(Environment.NewLine.Length);
		return sb.ToStringAndClear();
	}
}
