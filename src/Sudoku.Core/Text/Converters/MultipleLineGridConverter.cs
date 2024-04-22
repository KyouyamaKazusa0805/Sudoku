namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a converter that converts a <see cref="Grid"/> instance into an equivalent <see cref="string"/> representation
/// using multiple-line formatting rule.
/// </summary>
/// <param name="SubtleGridLines"><inheritdoc cref="PencilmarkingGridConverter.SubtleGridLines" path="/summary"/></param>
/// <param name="TreatValueAsGiven"><inheritdoc cref="PencilmarkingGridConverter.TreatValueAsGiven" path="/summary"/></param>
public sealed partial record MultipleLineGridConverter(bool SubtleGridLines = true, bool TreatValueAsGiven = false) :
	IConceptConverter<Grid>
{
	/// <summary>
	/// Indicates the zero character.
	/// </summary>
	private const char Zero = '0';

	/// <summary>
	/// Indicates the dot character.
	/// </summary>
	private const char Dot = '.';


	/// <summary>
	/// Indicates the default instance. The properties set are:
	/// <list type="bullet">
	/// <item><see cref="Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="SubtleGridLines"/>: <see langword="true"/></item>
	/// <item><see cref="TreatValueAsGiven"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static readonly MultipleLineGridConverter Default = new() { Placeholder = Dot };


	/// <summary>
	/// Indicates the placeholder of the grid text formatter.
	/// </summary>
	/// <value>The new placeholder text character to be set. The value must be <c>'.'</c> or <c>'0'</c>.</value>
	/// <returns>The placeholder text.</returns>
	[ImplicitField]
	public required char Placeholder
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _placeholder;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _placeholder = value switch
		{
			Zero or Dot => value,
			_ => throw new InvalidOperationException($"The placeholder character invalid; expected: '{Zero}' or '{Dot}'.")
		};
	}


	/// <inheritdoc/>
	public FuncRefReadOnly<Grid, string> Converter
		=> (ref readonly Grid grid) =>
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
		};
}
