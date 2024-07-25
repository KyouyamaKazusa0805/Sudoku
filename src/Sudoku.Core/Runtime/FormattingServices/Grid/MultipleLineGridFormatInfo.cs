namespace Sudoku.Runtime.FormattingServices;

/// <summary>
/// Represents a <see cref="GridFormatInfo"/> type that supports multiple formatting.
/// </summary>
public sealed partial class MultipleLineGridFormatInfo : GridFormatInfo
{
	[GeneratedRegex("""(\+?\d|\.)""", RegexOptions.Compiled, 5000)]
	private static partial Regex GridSusserDigitPattern { get; }

	[GeneratedRegex("""([\d\.\+]{9}(\r|\n|\r\n)){8}[\d\.\+]{9}""", RegexOptions.Compiled, 5000)]
	private static partial Regex GridSimpleMultilinePattern { get; }


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(GridFormatInfo) ? this : null;

	/// <inheritdoc/>
	public override MultipleLineGridFormatInfo Clone()
		=> new() { SubtleGridLines = SubtleGridLines, TreatValueAsGiven = TreatValueAsGiven };

	/// <inheritdoc/>
	protected internal override string FormatGrid(ref readonly Grid grid)
	{
		var t = grid.ToString(TreatValueAsGiven ? $"{Placeholder}!" : Placeholder.ToString());
		return new StringBuilder()
			.AppendLine(SubtleGridLines ? ".-------.-------.-------." : "+-------+-------+-------+")
			.Append("| ").Append(t[0]).Append(' ').Append(t[1]).Append(' ').Append(t[2])
			.Append(" | ").Append(t[3]).Append(' ').Append(t[4]).Append(' ').Append(t[5])
			.Append(" | ").Append(t[6]).Append(' ').Append(t[7]).Append(' ').Append(t[8])
			.AppendLine(" |")
			.Append("| ").Append(t[9]).Append(' ').Append(t[10]).Append(' ').Append(t[11])
			.Append(" | ").Append(t[12]).Append(' ').Append(t[13]).Append(' ').Append(t[14])
			.Append(" | ").Append(t[15]).Append(' ').Append(t[16]).Append(' ').Append(t[17])
			.AppendLine(" |")
			.Append("| ").Append(t[18]).Append(' ').Append(t[19]).Append(' ').Append(t[20])
			.Append(" | ").Append(t[21]).Append(' ').Append(t[22]).Append(' ').Append(t[23])
			.Append(" | ").Append(t[24]).Append(' ').Append(t[25]).Append(' ').Append(t[26])
			.AppendLine(" |")
			.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+")
			.Append("| ").Append(t[27]).Append(' ').Append(t[28]).Append(' ').Append(t[29])
			.Append(" | ").Append(t[30]).Append(' ').Append(t[31]).Append(' ').Append(t[32])
			.Append(" | ").Append(t[33]).Append(' ').Append(t[34]).Append(' ').Append(t[35])
			.AppendLine(" |")
			.Append("| ").Append(t[36]).Append(' ').Append(t[37]).Append(' ').Append(t[38])
			.Append(" | ").Append(t[39]).Append(' ').Append(t[40]).Append(' ').Append(t[41])
			.Append(" | ").Append(t[42]).Append(' ').Append(t[43]).Append(' ').Append(t[44])
			.AppendLine(" |")
			.Append("| ").Append(t[45]).Append(' ').Append(t[46]).Append(' ').Append(t[47])
			.Append(" | ").Append(t[48]).Append(' ').Append(t[49]).Append(' ').Append(t[50])
			.Append(" | ").Append(t[51]).Append(' ').Append(t[52]).Append(' ').Append(t[53])
			.AppendLine(" |")
			.AppendLine(SubtleGridLines ? ":-------+-------+-------:" : "+-------+-------+-------+")
			.Append("| ").Append(t[54]).Append(' ').Append(t[55]).Append(' ').Append(t[56])
			.Append(" | ").Append(t[57]).Append(' ').Append(t[58]).Append(' ').Append(t[59])
			.Append(" | ").Append(t[60]).Append(' ').Append(t[61]).Append(' ').Append(t[62])
			.AppendLine(" |")
			.Append("| ").Append(t[63]).Append(' ').Append(t[64]).Append(' ').Append(t[65])
			.Append(" | ").Append(t[66]).Append(' ').Append(t[67]).Append(' ').Append(t[68])
			.Append(" | ").Append(t[69]).Append(' ').Append(t[70]).Append(' ').Append(t[71])
			.AppendLine(" |")
			.Append("| ").Append(t[72]).Append(' ').Append(t[73]).Append(' ').Append(t[74])
			.Append(" | ").Append(t[75]).Append(' ').Append(t[76]).Append(' ').Append(t[77])
			.Append(" | ").Append(t[78]).Append(' ').Append(t[79]).Append(' ').Append(t[80])
			.AppendLine(" |")
			.Append(SubtleGridLines ? "'-------'-------'-------'" : "+-------+-------+-------+")
			.ToString();
	}

	/// <inheritdoc/>
	protected internal override Grid ParseGrid(string str)
	{
		if (RemoveGridLines)
		{
			return GridSimpleMultilinePattern.Match(str) is not { Success: true, Value: var match }
				? Grid.Undefined
				: new SusserGridFormatInfo().ParseGrid(new(from @char in match where @char is not ('\r' or '\n') select @char));
		}
		else
		{
			var matches = from match in GridSusserDigitPattern.Matches(str) select match.Value;
			if (matches.Length is not (var length and (81 or 85)))
			{
				// Subtle grid outline will bring 2 '.'s on first line of the grid.
				return Grid.Undefined;
			}

			var result = Grid.Empty;
			for (var i = 0; i < 81; i++)
			{
				switch (matches[length - 81 + i])
				{
					case [var match and not ('.' or '0')]:
					{
						result.SetDigit(i, match - '1');
						result.SetState(i, CellState.Given);
						break;
					}
					case { Length: 1 }:
					{
						continue;
					}
					case [_, var match]:
					{
						if (match is '.' or '0')
						{
							// '+0' or '+.'? Invalid combination.
							return Grid.Undefined;
						}

						result.SetDigit(i, match - '1');
						result.SetState(i, CellState.Modifiable);
						break;
					}
					default:
					{
						// The sub-match contains more than 2 characters or empty string,
						// which is invalid.
						return Grid.Undefined;
					}
				}
			}
			return result;
		}
	}
}
