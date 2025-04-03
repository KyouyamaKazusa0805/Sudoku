namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents extra options that formats a <see cref="Grid"/> instance, or parses into a <see cref="Grid"/> instance.
/// </summary>
/// <typeparam name="TGrid">The type of grid.</typeparam>
/// <seealso cref="Grid"/>
/// <seealso cref="NumberFormatInfo"/>
public abstract class GridFormatInfo<TGrid> : FormatInfo<TGrid> where TGrid : unmanaged, IGrid<TGrid>
{
	/// <summary>
	/// Indicates the table of format and creator.
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Advanced)]
	private static readonly (string?[] FormatChecker, Func<GridFormatInfo<Grid>> Creator)[] ValuesRouter = [
		([null, "."], static () => new SusserGridFormatInfo<Grid>()),
		(["0"], static () => new SusserGridFormatInfo<Grid> { Placeholder = '0' }),
		(["0+", "+0"], static () => new SusserGridFormatInfo<Grid> { Placeholder = '0', WithModifiables = true }),
		(["+", ".+", "+."], static () => new SusserGridFormatInfo<Grid> { WithModifiables = true }),
		(["+:", "+.:", ".+:", "#", "#."], static () => new SusserGridFormatInfo<Grid>{ WithCandidates = true, WithModifiables = true }),
		(["^+:", "^:+", "^.+:", "^#", "^#."], static () => new SusserGridFormatInfo<Grid> { WithCandidates = true, WithModifiables = true, NegateEliminationsTripletRule = true }),
		(["0+:", "+0:", "#0"], static () => new SusserGridFormatInfo<Grid> { WithCandidates = true, WithModifiables = true, Placeholder = '0' }),
		(["^0+:", "^+0:", "^#0"], static () => new SusserGridFormatInfo<Grid> { WithCandidates = true, WithModifiables = true, Placeholder = '0', NegateEliminationsTripletRule = true }),
		([":", ".:"], static () => new SusserGridFormatInfo<Grid> { WithCandidates = true, OnlyEliminations = true }),
		(["0:"], static () => new SusserGridFormatInfo<Grid> { WithCandidates = true, OnlyEliminations = true, Placeholder = '0' }),
		(["!", ".!", "!."], static () => new SusserGridFormatInfo<Grid> { TreatValueAsGiven = true, WithModifiables = true }),
		(["0!", "!0"], static () => new SusserGridFormatInfo<Grid> { TreatValueAsGiven = true, WithModifiables = true, Placeholder = '0' }),
		([".!:", "!.:"], static () => new SusserGridFormatInfo<Grid> { WithCandidates = true, WithModifiables = true, TreatValueAsGiven = true }),
		(["^.!:", "^!.:"], static () => new SusserGridFormatInfo<Grid> { WithCandidates = true, TreatValueAsGiven = true, NegateEliminationsTripletRule = true }),
		(["0!:", "!0:"], static () => new SusserGridFormatInfo<Grid> { WithCandidates = true, WithModifiables = true, TreatValueAsGiven = true, Placeholder = '0' }),
		(["^0!:", "^!0:"], static () => new SusserGridFormatInfo<Grid> { WithCandidates = true, WithModifiables = true, TreatValueAsGiven = true, NegateEliminationsTripletRule = true, Placeholder = '0' }),
		([".*", "*."], static () => new SusserGridFormatInfo<Grid> { ShortenSusser = true }),
		(["0*", "*0"], static () => new SusserGridFormatInfo<Grid> { Placeholder = '0', ShortenSusser = true }),
		(["@", "@."], static () => new MultipleLineGridFormatInfo()),
		(["@*", "@.*", "@*."], static () => new MultipleLineGridFormatInfo { SubtleGridLines = false }),
		(["@0"], static () => new MultipleLineGridFormatInfo { Placeholder = '0' }),
		(["@0!", "@!0"], static () => new MultipleLineGridFormatInfo { Placeholder = '0', TreatValueAsGiven = true }),
		(["@0*", "@*0"], static () => new MultipleLineGridFormatInfo { Placeholder = '0', SubtleGridLines = false }),
		(["@!", "@.!", "@!."], static () => new MultipleLineGridFormatInfo { TreatValueAsGiven = true }),
		(["@!*", "@*!"], static () => new MultipleLineGridFormatInfo { TreatValueAsGiven = true, SubtleGridLines = false }),
		(["@:"], static () => new PencilmarkGridFormatInfo()),
		(["@*:", "@:*"], static () => new PencilmarkGridFormatInfo { SubtleGridLines = false }),
		(["@:!", "@!:"], static () => new PencilmarkGridFormatInfo { TreatValueAsGiven = true }),
		(["@!*:", "@*!:", "@!:*", "@*:!", "@:!*", "@:*!"], static () => new PencilmarkGridFormatInfo { TreatValueAsGiven = true, SubtleGridLines = false }),
		(["~."], static () => new SukakuGridFormatInfo()),
		(["~", "~0"], static () => new SukakuGridFormatInfo { Placeholder = '0' }),
		(["@~", "~@", "@~.", "@.~", "~@.", "~.@"], static () => new SukakuGridFormatInfo { Multiline = true }),
		(["@~0", "@0~", "~@0", "~0@"], static () => new SukakuGridFormatInfo { Multiline = true, Placeholder = '0' })
	];


	/// <summary>
	/// <para>Indicates whether the formatter will reserve candidates as pre-elimination.</para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool WithCandidates { get; set; }

	/// <summary>
	/// <para>
	/// Indicates whether the formatter will output and distinct modifiable and given digits.
	/// If so, the modifiable digits will be displayed as <c>+digit</c>, where <c>digit</c> will be replaced
	/// with the real digit number (from 1 to 9).
	/// </para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool WithModifiables { get; set; }

	/// <summary>
	/// <para>
	/// Indicates whether the parser will use shorten mode to parse a susser format grid.
	/// If the value is <see langword="true"/>, the parser will omit the continuous empty notation
	/// <c>.</c>s or <c>0</c>s to a <c>*</c>.
	/// </para>
	/// <para>
	/// This option will omit the continuous empty cells to a <c>*</c> in a single line. For example, the code
	/// <code><![CDATA[
	/// 080630040200085009090000081000300800000020000006001000970000030400850007010094050
	/// ]]></code>
	/// will be displayed as
	/// <code><![CDATA[
	/// 08063*40,2*85009,09*81,*300800,*2*,006001*,97*30,40085*7,01*94050
	/// ]]></code>
	/// via this option.
	/// We use the colon <c>,</c> to separate each line of 9 numbers, and then omit the most continuous empty cells to a <c>*</c>.
	/// </para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool ShortenSusser { get; set; }

	/// <summary>
	/// <para>
	/// Indicates whether the parser will negate the rule, treating all digits as candidates existing in the grid
	/// instead of removed ones.
	/// </para>
	/// <para>The default value is <see langword="false"/>.</para>
	/// </summary>
	public bool NegateEliminationsTripletRule { get; set; }

	/// <summary>
	/// Indicates whether the formatter will treat all values as givens, regardless of its value state.
	/// </summary>
	public bool TreatValueAsGiven { get; set; }

	/// <summary>
	/// Indicates whether the formatter will subtle grid lines to make a good-look.
	/// </summary>
	public bool SubtleGridLines { get; set; } = true;

	/// <summary>
	/// Indicates whether the formatter will use compatible mode to output grid values.
	/// </summary>
	public bool IsCompatibleMode { get; set; }

	/// <summary>
	/// Indicates whether the formatter will handle the value with multiple-line mode.
	/// </summary>
	public bool Multiline { get; set; }

	/// <summary>
	/// Indicates whether the parsing operation will use simple way, removing all grid lines for multi-line formatting.
	/// </summary>
	public bool RemoveGridLines { get; set; }

	/// <summary>
	/// Indicates whether the formatting operation will output for eliminations.
	/// </summary>
	public bool OnlyEliminations { get; set; }

	/// <summary>
	/// Indicates the placeholder of the grid text formatter.
	/// </summary>
	/// <value>The new placeholder text character to be set. The value must be <c>'.'</c> or <c>'0'</c>.</value>
	/// <returns>The placeholder text.</returns>
	public char Placeholder { get; init; } = '.';

	/// <summary>
	/// Indicates the separator used. By default, it's comma <c>", "</c>.
	/// </summary>
	public string Separator { get; set; } = ", ";


	/// <summary>
	/// Gets a read-only <see cref="IFormatProvider"/> object that is culture-independent (invariant).
	/// </summary>
	public static IFormatProvider InvariantCulture => GetInstance(CultureInfo.InvariantCulture)!;

	/// <summary>
	/// Gets a <see cref="IFormatProvider"/> that formats values based on the current culture.
	/// </summary>
	public static IFormatProvider? CurrentCulture => GetInstance(CultureInfo.CurrentCulture);

	/// <summary>
	/// Represents CSV format rule.
	/// </summary>
	public static IFormatProvider CsvFormat => new CsvGridFormatInfo();

	/// <summary>
	/// Represents inline Susser format rule.
	/// </summary>
	public static IFormatProvider InlineSusserFormat => new InlineSusserGridFormatInfo();

	/// <summary>
	/// Represents mask format rule.
	/// </summary>
	public static IFormatProvider MaskFormat => new MaskGridFormatInfo();

	/// <summary>
	/// Represents multiple line format rule.
	/// </summary>
	public static IFormatProvider MultipleLineFormat => new MultipleLineGridFormatInfo();

	/// <summary>
	/// Represents open sudoku format rule.
	/// </summary>
	public static IFormatProvider OpenSudokuFormat => new OpenSudokuGridFormatInfo();

	/// <summary>
	/// Represents pencilmark format rule.
	/// </summary>
	public static IFormatProvider PencilmarkFormat => new PencilmarkGridFormatInfo();

	/// <summary>
	/// Represents Sukaku format rule.
	/// </summary>
	public static IFormatProvider SukakuFormat => new SukakuGridFormatInfo();

	/// <summary>
	/// Represents standard format tule (Susser rule).
	/// </summary>
	public static IFormatProvider SusserFormat => new SusserGridFormatInfo<Grid>();

	/// <summary>
	/// Represents standard format rule (Susser rule), to parse into a <see cref="MarkerGrid"/> instance.
	/// </summary>
	public static IFormatProvider SusserFormatMarkerGrid => new SusserGridFormatInfo<MarkerGrid>();


	/// <summary>
	/// Gets the <see cref="GridFormatInfo{TGrid}"/> of <see cref="Grid"/>
	/// associated with the specified <see cref="IFormatProvider"/>.
	/// </summary>
	/// <param name="formatProvider">The <see cref="IFormatProvider"/> used to get the <see cref="GridFormatInfo{TGrid}"/>.</param>
	/// <returns>
	/// The <see cref="GridFormatInfo{TGrid}"/> of <see cref="Grid"/> associated with the specified <see cref="IFormatProvider"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GridFormatInfo<Grid>? GetInstance(IFormatProvider? formatProvider)
		=> GetInstance(
			formatProvider switch
			{
				CultureInfo c => c.Name switch { SR.EnglishLanguage => "@:", SR.ChineseLanguage => ".", _ => "#" },
				_ => "#"
			}
		);

	/// <summary>
	/// Creates a <see cref="GridFormatInfo{TGrid}"/> instance that holds the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>A valid <see cref="GridFormatInfo{TGrid}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static GridFormatInfo<Grid>? GetInstance(string? format)
	{
		var p = Array.FindIndex(ValuesRouter, pair => Array.IndexOf(pair.FormatChecker, format) != -1);
		return p == -1 ? null : ValuesRouter[p].Creator();
	}
}
