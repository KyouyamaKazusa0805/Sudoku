namespace Sudoku.Concepts;

/// <summary>
/// Represents extra options that formats a <see cref="Grid"/> instance, or parses into a <see cref="Grid"/> instance.
/// </summary>
/// <seealso cref="Grid"/>
[Equals(OtherModifiers = "sealed")]
[GetHashCode(GetHashCodeBehavior.MakeAbstract)]
[EqualityOperators]
public abstract partial class GridFormatInfo :
	IEquatable<GridFormatInfo>,
	IEqualityOperators<GridFormatInfo, GridFormatInfo, bool>,
	IFormatProvider
{
	/// <summary>
	/// Indicates the table of format and creator.
	/// </summary>
	private static readonly (string?[] FormatChecker, Func<GridFormatInfo> Creator)[] ValuesRouter = [
		([null, "."], static () => new SusserGridFormatInfo()),
		(["0"], static () => new SusserGridFormatInfo { Placeholder = '0' }),
		(["0+", "+0"], static () => new SusserGridFormatInfo { Placeholder = '0', WithModifiables = true }),
		(["+", ".+", "+."], static () => new SusserGridFormatInfo { WithModifiables = true }),
		(["+:", "+.:", ".+:", "#", "#."], static () => new SusserGridFormatInfo { WithCandidates = true, WithModifiables = true }),
		(["^+:", "^:+", "^.+:", "^#", "^#."], static () => new SusserGridFormatInfo { WithCandidates = true, WithModifiables = true, NegateEliminationsTripletRule = true }),
		(["0+:", "+0:", "#0"], static () => new SusserGridFormatInfo { WithCandidates = true, WithModifiables = true, Placeholder = '0' }),
		(["^0+:", "^+0:", "^#0"], static () => new SusserGridFormatInfo { WithCandidates = true, WithModifiables = true, Placeholder = '0', NegateEliminationsTripletRule = true }),
		([":", ".:"], static () => new SusserGridFormatInfo { WithCandidates = true, OnlyEliminations = true }),
		(["0:"], static () => new SusserGridFormatInfo { WithCandidates = true, OnlyEliminations = true, Placeholder = '0' }),
		(["!", ".!", "!."], static () => new SusserGridFormatInfo { TreatValueAsGiven = true, WithModifiables = true }),
		(["0!", "!0"], static () => new SusserGridFormatInfo { TreatValueAsGiven = true, WithModifiables = true, Placeholder = '0' }),
		([".!:", "!.:"], static () => new SusserGridFormatInfo { WithCandidates = true, WithModifiables = true, TreatValueAsGiven = true }),
		(["^.!:", "^!.:"], static () => new SusserGridFormatInfo { WithCandidates = true, TreatValueAsGiven = true, NegateEliminationsTripletRule = true }),
		(["0!:", "!0:"], static () => new SusserGridFormatInfo { WithCandidates = true, WithModifiables = true, TreatValueAsGiven = true, Placeholder = '0' }),
		(["^0!:", "^!0:"], static () => new SusserGridFormatInfo { WithCandidates = true, WithModifiables = true, TreatValueAsGiven = true, NegateEliminationsTripletRule = true, Placeholder = '0' }),
		([".*", "*."], static () => new SusserGridFormatInfo { ShortenSusser = true }),
		(["0*", "*0"], static () => new SusserGridFormatInfo { Placeholder = '0', ShortenSusser = true }),
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
	/// Indicaets the separator used. By default it's comma <c>", "</c>.
	/// </summary>
	public string Separator { get; set; } = ", ";


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public abstract object? GetFormat(Type? formatType);

	/// <inheritdoc/>
	public abstract bool Equals([NotNullWhen(true)] GridFormatInfo? other);

	/// <summary>
	/// Creates a copy of the current instance.
	/// </summary>
	/// <returns>A new instance whose internal values are equal to the current instance.</returns>
	public abstract GridFormatInfo Clone();

	/// <summary>
	/// Try to format the current grid into a valid string result.
	/// </summary>
	/// <param name="grid">The grid to be formatted.</param>
	/// <returns>The <see cref="string"/> representation of the argument <paramref name="grid"/>.</returns>
	protected internal abstract string FormatGrid(ref readonly Grid grid);

	/// <summary>
	/// Try to parse the specified <see cref="string"/> instance into a valid <see cref="Grid"/>.
	/// </summary>
	/// <param name="str">The string value to be parsed.</param>
	/// <returns>The <see cref="Grid"/> as the result.</returns>
	protected internal abstract Grid ParseGrid(string str);


	/// <summary>
	/// Creates a <see cref="GridFormatInfo"/> instance that holds the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>A valid <see cref="GridFormatInfo"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal static GridFormatInfo? Create(string? format)
	{
		var p = Array.FindIndex(ValuesRouter, pair => Array.IndexOf(pair.FormatChecker, format) != -1);
		return p == -1 ? null : ValuesRouter[p].Creator();
	}
}
