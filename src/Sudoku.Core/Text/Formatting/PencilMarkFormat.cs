using System.Algorithm;
using System.Text;
using Sudoku.Runtime.MaskServices;
using static System.Math;

namespace Sudoku.Text.Formatting;

/// <summary>
/// Represents with a pencil-marked formatter.
/// </summary>
/// <param name="SubtleGridLines">
/// <para>Indicates whether need to handle all grid outlines while outputting.</para>
/// <para>The default value is <see langword="true"/>.</para>
/// </param>
/// <param name="TreatValueAsGiven">
/// <para>
/// Indicates the output will treat modifiable values as given ones.
/// If the output is single line, the output will remove all plus marks '+'.
/// If the output is multi-line, the output will use '<c><![CDATA[<digit>]]></c>' instead
/// of '<c>*digit*</c>'.
/// </para>
/// <para>
/// The value has 3 possible cases:
/// <list type="table">
/// <item>
/// <term><see langword="true"/></term>
/// <description>
/// The formatter will treat all value cells as given one, no matter what kind of value cell it is, given or modifiable.
/// </description>
/// </item>
/// <item>
/// <term><see langword="false"/></term>
/// <description>The formatter will tell with givens and modifiables.</description>
/// </item>
/// <item>
/// <term><see langword="null"/></term>
/// <description>
/// The formatter will <b>not</b> check its value states. It will be displayed and handled as candidate cells,
/// using a single digit to display the cell.
/// </description>
/// </item>
/// </list>
/// </para>
/// <para>The default value is <see langword="false"/>.</para>
/// </param>
public sealed record PencilMarkFormat(bool SubtleGridLines = true, bool? TreatValueAsGiven = false) : IGridFormatter
{
	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="SubtleGridLines"/>: <see langword="true"/></item>
	/// <item><see cref="TreatValueAsGiven"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static readonly PencilMarkFormat Default = new();


	/// <inheritdoc/>
	static IGridFormatter IGridFormatter.Instance => Default;


	/// <inheritdoc/>
	public unsafe string ToString(scoped in Grid grid)
	{
		// Step 1: gets the candidates information grouped by columns.
		var valuesByColumn = CreateTempDictionary();
		var valuesByRow = CreateTempDictionary();

		for (var i = 0; i < 81; i++)
		{
			var value = grid[i];
			valuesByRow[i / 9].Add(value);
			valuesByColumn[i % 9].Add(value);
		}

		// Step 2: gets the maximal number of candidates in a cell, used for aligning by columns.
		const int bufferLength = 9;
		var maxLengths = stackalloc int[bufferLength];
		InitBlock(maxLengths, 0, sizeof(int) * bufferLength);

		foreach (var (i, _) in valuesByColumn)
		{
			var maxLength = maxLengths + i;

			// Iteration on row index.
			for (var j = 0; j < 9; j++)
			{
				// Gets the number of candidates.
				var candidatesCount = 0;
				var value = valuesByColumn[i][j];

				// Iteration on each candidate.
				// Counts the number of candidates.
				candidatesCount += PopCount((uint)value);

				// Compares the values.
				var comparer = Max(
					candidatesCount,
					MaskOperations.MaskToCellState(value) switch
					{
						// The output will be '<digit>' and consist of 3 characters.
						CellState.Given => Max(candidatesCount, TreatValueAsGiven is null ? 1 : 3),

						// The output will be '*digit*' and consist of 3 characters.
						CellState.Modifiable => Max(candidatesCount, TreatValueAsGiven is null ? 1 : 3),

						// Normal output: 'series' (at least 1 character).
						_ => candidatesCount
					}
				);
				if (comparer > *maxLength)
				{
					*maxLength = comparer;
				}
			}
		}

		// Step 3: outputs all characters.
		scoped var sb = new StringHandler();
		for (var i = 0; i < 13; i++)
		{
			switch (i)
			{
				case 0: // Print tabs of the first line.
				{
					if (SubtleGridLines)
					{
						PrintTabLines(ref sb, '.', '.', '-', maxLengths);
					}
					else
					{
						PrintTabLines(ref sb, '+', '+', '-', maxLengths);
					}
					break;
				}
				case 4 or 8: // Print tabs of mediate lines.
				{
					if (SubtleGridLines)
					{
						PrintTabLines(ref sb, ':', '+', '-', maxLengths);
					}
					else
					{
						PrintTabLines(ref sb, '+', '+', '-', maxLengths);
					}
					break;
				}
				case 12: // Print tabs of the foot line.
				{
					if (SubtleGridLines)
					{
						PrintTabLines(ref sb, '\'', '\'', '-', maxLengths);
					}
					else
					{
						PrintTabLines(ref sb, '+', '+', '-', maxLengths);
					}
					break;
				}
				default: // Print values and tabs.
				{
					DefaultPrinting(ref sb, valuesByRow[Sequences.A057353(i)], '|', '|', maxLengths);

					break;
				}
			}
		}

		// The last step: returns the value.
		sb.RemoveFromEnd(Environment.NewLine.Length);
		return sb.ToString();
	}

	/// <summary>
	/// Default printing method.
	/// </summary>
	private unsafe void DefaultPrinting(scoped ref StringHandler sb, IList<Mask> valuesByRow, char c1, char c2, int* maxLengths)
	{
		sb.Append(c1);
		PrintValues(ref sb, valuesByRow, 0, 2, maxLengths);
		sb.Append(c2);
		PrintValues(ref sb, valuesByRow, 3, 5, maxLengths);
		sb.Append(c2);
		PrintValues(ref sb, valuesByRow, 6, 8, maxLengths);
		sb.Append(c1);
		sb.AppendLine();
	}

	/// <summary>
	/// Print values.
	/// </summary>
	private unsafe void PrintValues(scoped ref StringHandler sb, IList<Mask> valuesByRow, int start, int end, int* maxLengths)
	{
		sb.Append(' ');
		for (var i = start; i <= end; i++)
		{
			// Get digit.
			var value = valuesByRow[i];
			var state = MaskOperations.MaskToCellState(value);

			value &= Grid.MaxCandidatesMask;
			var d = value == 0 ? -1 : (state != CellState.Empty ? TrailingZeroCount(value) : -1) + 1;
			var s = (state, TreatValueAsGiven) switch
			{
				(CellState.Given, not null) or (CellState.Modifiable, true) => $"<{d}>",
				(CellState.Modifiable, false) => $"*{d}*",
				(CellState.Given or CellState.Modifiable, null) => d.ToString(),
				_ => appendingMask(value)
			};

			sb.Append(s.PadRight(maxLengths[i]));
			sb.Append(i != end ? "  " : " ");
		}


		static string appendingMask(Mask value)
		{
			scoped var innerSb = new StringHandler(9);
			foreach (var z in value)
			{
				innerSb.Append(z + 1);
			}

			return innerSb.ToStringAndClear();
		}
	}

	/// <summary>
	/// Print tab lines.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static unsafe void PrintTabLines(scoped ref StringHandler sb, char c1, char c2, char fillingChar, int* m)
	{
		sb.Append(c1);
		sb.Append(string.Empty.PadRight(m[0] + m[1] + m[2] + 6, fillingChar));
		sb.Append(c2);
		sb.Append(string.Empty.PadRight(m[3] + m[4] + m[5] + 6, fillingChar));
		sb.Append(c2);
		sb.Append(string.Empty.PadRight(m[6] + m[7] + m[8] + 6, fillingChar));
		sb.Append(c1);
		sb.AppendLine();
	}

	/// <summary>
	/// Creates a dictionary.
	/// </summary>
	/// <returns>The dictionary instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static Dictionary<int, List<Mask>> CreateTempDictionary()
		=> new() { { 0, new() }, { 1, new() }, { 2, new() }, { 3, new() }, { 4, new() }, { 5, new() }, { 6, new() }, { 7, new() }, { 8, new() } };
}
