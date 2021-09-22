namespace Sudoku.UI.Data;

/// <summary>
/// Defines a <see cref="UIColor"/> instance that can converts to a <see cref="Color"/>.
/// </summary>
/// <param name="A">The alpha value.</param>
/// <param name="R">The red value.</param>
/// <param name="G">The green value.</param>
/// <param name="B">The blue value.</param>
[AutoDeconstruct(nameof(A), nameof(RgbColorInstance))]
[AutoDeconstruct(nameof(R), nameof(G), nameof(B))]
public readonly partial record struct UIColor(byte A, byte R, byte G, byte B) : IValueEquatable<UIColor>, IFormattable, IParsable<UIColor>, IMinMaxValue<UIColor>
{
	/// <summary>
	/// Indicates the min value of the <see cref="UIColor"/> instance.
	/// </summary>
	public static readonly UIColor MinValue;

	/// <summary>
	/// Indicates the max value of the <see cref="UIColor"/> instance.
	/// </summary>
	public static readonly UIColor MaxValue = new(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

	/// <summary>
	/// Indicates the match timeout.
	/// </summary>
	private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(5);

	/// <summary>
	/// Indicates the regular expression instance that matches an ARGB format color.
	/// </summary>
	/// <remarks>
	/// All possible match types:
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <description>Example</description>
	/// </listheader>
	/// <item>
	/// <term>RGB repeat</term>
	/// <description>#CDE (i.e. #FFCCDDEE)</description>
	/// </item>
	/// <item>
	/// <term>RGB repeat with alpha value</term>
	/// <description>#FFCDE (i.e. #FFCCDDEE)</description>
	/// </item>
	/// <item>
	/// <term>RGB</term>
	/// <description>#CCDDEE (i.e. #CCDDEE)</description>
	/// </item>
	/// <item>
	/// <term>ARGB</term>
	/// <description>#FFCCDDEE</description>
	/// </item>
	/// </list>
	/// </remarks>
	private static readonly Regex ArgbValidator = new(
		@"((?<=#)[A-Fa-f\d]{3}$|(?<=#)[A-Fa-f\d]{5}$|(?<=#)[A-Fa-f\d]{6}$|(?<=#)[A-Fa-f\d]{8}$)",
		RegexOptions.Compiled,
		MatchTimeout
	);

	/// <summary>
	/// Indicates the regular expression instance that matches a bracket format color.
	/// </summary>
	/// <remarks>
	/// All possible match types:
	/// <list type="table">
	/// <listheader>
	/// <term>Type</term>
	/// <description>Example</description>
	/// </listheader>
	/// <item>
	/// <term>RGB</term>
	/// <description>(255, 255, 255) (i.e. #FFFFFFFF)</description>
	/// </item>
	/// <item>
	/// <term>ARGB</term>
	/// <description>(255, 255, 255, 255) (i.e. #FFFFFFFF)</description>
	/// </item>
	/// </list>
	/// </remarks>
	private static readonly Regex BracketValidator = new(
		@"((?<=\()(\d|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5]\s*)(,\s*(\d|[1-9]\d|1\d{2}|2[0-4]\d|25[0-5])\s*){2,3}(?=\)))",
		RegexOptions.Compiled,
		MatchTimeout
	);


	/// <summary>
	/// Initializes a <see cref="UIColor"/> instance with the specified mask.
	/// </summary>
	/// <param name="mask">The mask.</param>
	public UIColor(int mask)
	: this(
		(byte)(mask >> 24 & byte.MaxValue),
		(byte)(mask >> 16 & byte.MaxValue),
		(byte)(mask >> 8 & byte.MaxValue),
		(byte)(mask & byte.MaxValue)
	)
	{
	}

	/// <summary>
	/// Initializes a <see cref="UIColor"/> instance with the specified alpha value
	/// and the base <see cref="Color"/> instance.
	/// </summary>
	/// <param name="alpha">The alpha value.</param>
	/// <param name="baseColor">The base color.</param>
	public UIColor(byte alpha, Color baseColor) : this(alpha, baseColor.R, baseColor.G, baseColor.B)
	{
	}

	/// <summary>
	/// Initializes a <see cref="UIColor"/> instance with the specified alpha value
	/// and the base <see cref="UIColor"/> instance.
	/// </summary>
	/// <param name="alpha">The alpha value.</param>
	/// <param name="baseColor">The base color.</param>
	public UIColor(byte alpha, UIColor baseColor) : this(alpha, baseColor.R, baseColor.G, baseColor.B)
	{
	}

	/// <summary>
	/// Initializes a <see cref="UIColor"/> instance with the specified R, G, B value.
	/// </summary>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	public UIColor(byte r, byte g, byte b) : this(byte.MaxValue, r, g, b)
	{
	}


	/// <summary>
	/// Indicates the mask.
	/// </summary>
	private int Mask => A << 24 | R << 16 | G << 8 | B;

	/// <summary>
	/// Indicates the <see cref="UIColor"/> instance only contains the R, G, B values.
	/// </summary>
	private UIColor RgbColorInstance => new(R, G, B);

	/// <inheritdoc/>
	static UIColor IMinMaxValue<UIColor>.MinValue => MinValue;

	/// <inheritdoc/>
	static UIColor IMinMaxValue<UIColor>.MaxValue => MaxValue;


	/// <inheritdoc/>
	public bool Equals(UIColor other) => Mask == other.Mask;

	/// <inheritdoc/>
	public override int GetHashCode() => Mask;

	/// <inheritdoc/>
	public override string ToString() => ToString(null, null);

	/// <summary>
	/// Formats the value of the current instance using the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The value of the current instance in the specified format.</returns>
	public string ToString(string? format) => ToString(format, null);

	/// <inheritdoc/>
	/// <exception cref="FormatException">Throws when the <paramref name="format"/> is invalid.</exception>
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		formatProvider.HasFormatted(this, format, out string? result) ? result : format switch
		{
			null or "D" => Mask.ToString(), // Decimal type.
			"F" => $"A: {A}, R: {R}, G: {G}, B: {B}", // Full format type.
			_ => throw new FormatException("The specified format is invalid.")
		};

	/// <inheritdoc/>
	bool IValueEquatable<UIColor>.Equals(in UIColor other) => Equals(other);


	/// <inheritdoc/>
	public static UIColor Parse(string? str)
	{
		return parseArgb(str ?? throw new ArgumentNullException(nameof(str)), out var resultArgb)
			? resultArgb
			: parseBracket(str, out var resultBracket)
				? resultBracket
				: throw new FormatException("The specified string is invalid to parse.");


		static bool parseArgb(string str, out UIColor result)
		{
			if (ArgbValidator.Match(str) is not { Success: true, Value: { Length: var length } value } match)
			{
				result = MinValue;
				return false;
			}

			Func<string, int, byte> c = Convert.ToByte;

			(result, bool @return) = length switch
			{
				3 => (new(255, d(value[0], 2), d(value[1], 2), d(value[2], 2)), true),
				5 => (new(c(value[..2], 2), d(value[2], 2), d(value[3], 2), d(value[4], 2)), true),
				6 => (new(255, c(value[..2], 2), c(value[2..4], 2), c(value[4..], 2)), true),
				8 => (new(c(value[..2], 2), c(value[2..4], 2), c(value[4..6], 2), c(value[6..], 2)), true),
				_ => (MinValue, false)
			};

			return @return;


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			static byte d(char ch, int @base) => ch switch
			{
				>= '0' and <= '9' => (byte)(ch - '0'),
				'A' or 'a' => 10,
				'B' or 'b' => 11,
				'C' or 'c' => 12,
				'D' or 'd' => 13,
				'E' or 'e' => 14,
				'F' or 'f' => 15
			};
		}

		static bool parseBracket(string str, out UIColor result)
		{
			if (BracketValidator.Match(str) is not { Success: true } match)
			{
				result = MinValue;
				return false;
			}

			Func<string, byte> p = byte.Parse;

			string[] split = str.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			(result, bool @return) = split.Length switch
			{
				3 => (new(p(split[0]), p(split[1]), p(split[2])), true),
				4 => (new(p(split[0]), p(split[1]), p(split[2]), p(split[3])), true),
				_ => (MinValue, false)
			};

			return @return;
		}
	}

	/// <inheritdoc/>
	public static bool TryParse([NotNullWhen(true)] string? str, out UIColor result)
	{
		try
		{
			result = Parse(str);
			return true;
		}
		catch (Exception ex) when (ex is FormatException or ArgumentNullException)
		{
			result = MinValue;
			return false;
		}
	}


	/// <summary>
	/// Reverses a <see cref="UIColor"/>, that converts the base color to the reverse one.
	/// </summary>
	/// <param name="baseColor">The base color.</param>
	/// <returns>The <see cref="UIColor"/> result.</returns>
	public static UIColor operator ~(UIColor baseColor) =>
		new(
			(byte)(byte.MaxValue - baseColor.A),
			(byte)(byte.MaxValue - baseColor.R),
			(byte)(byte.MaxValue - baseColor.G),
			(byte)(byte.MaxValue - baseColor.B)
		);


	/// <summary>
	/// Implicit cast from <see cref="UIColor"/> to <see cref="Color"/>.
	/// </summary>
	/// <param name="color">The <see cref="UIColor"/> instance.</param>
	public static implicit operator Color(UIColor color) => Color.FromArgb(color.A, color.R, color.G, color.B);

	/// <summary>
	/// Explicit cast from <see cref="Color"/> to <see cref="UIColor"/>.
	/// </summary>
	/// <param name="color">The <see cref="Color"/> instance.</param>
	public static explicit operator UIColor(Color color) => new(color.A, color.R, color.G, color.B);
}
