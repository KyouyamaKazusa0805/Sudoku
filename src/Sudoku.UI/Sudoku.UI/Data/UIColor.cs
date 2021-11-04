extern alias extended;

namespace Sudoku.UI.Data;

/// <summary>
/// Defines a <see cref="UIColor"/> instance that can converts to a <see cref="Color"/>.
/// </summary>
/// <param name="A">The alpha value.</param>
/// <param name="R">The red value.</param>
/// <param name="G">The green value.</param>
/// <param name="B">The blue value.</param>
[AutoDeconstructLambda(nameof(A), nameof(RgbColorInstance))]
[AutoDeconstruct(nameof(R), nameof(G), nameof(B))]
public readonly partial record struct UIColor(byte A, byte R, byte G, byte B) : IValueEquatable<UIColor>, IFormattable, extended::System.IParseable<UIColor>, IMinMaxValue<UIColor>, IJsonSerializable<UIColor, UIColor.JsonConverter>
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UIColor(byte alpha, Color baseColor) : this(alpha, baseColor.R, baseColor.G, baseColor.B)
	{
	}

	/// <summary>
	/// Initializes a <see cref="UIColor"/> instance with the specified alpha value
	/// and the base <see cref="UIColor"/> instance.
	/// </summary>
	/// <param name="alpha">The alpha value.</param>
	/// <param name="baseColor">The base color.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UIColor(byte alpha, UIColor baseColor) : this(alpha, baseColor.R, baseColor.G, baseColor.B)
	{
	}

	/// <summary>
	/// Initializes a <see cref="UIColor"/> instance with the specified R, G, B value.
	/// </summary>
	/// <param name="r">The red value.</param>
	/// <param name="g">The green value.</param>
	/// <param name="b">The blue value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UIColor(byte r, byte g, byte b) : this(byte.MaxValue, r, g, b)
	{
	}

	/// <summary>
	/// Initializes a <see cref="UIColor"/> instance with the specified R, G and B weights.
	/// </summary>
	/// <param name="rWeight">The red weight. The argument must be between 0 and 1.</param>
	/// <param name="gWeight">The green weight. The argument must be between 0 and 1.</param>
	/// <param name="bWeight">The blue weight. The argument must be between 0 and 1.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when one argument of <paramref name="rWeight"/>, <paramref name="gWeight"/>
	/// and <paramref name="bWeight"/> isn't between 0 and 1.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UIColor(double rWeight, double gWeight, double bWeight)
	: this(
		ArgbArgCheck(rWeight, nameof(rWeight)),
		ArgbArgCheck(gWeight, nameof(gWeight)),
		ArgbArgCheck(bWeight, nameof(bWeight))
	)
	{
	}

	/// <summary>
	/// Initializes a <see cref="UIColor"/> instance with the specified A, R, G and B weights.
	/// </summary>
	/// <param name="aWeight">The alpha weight. The argument must be between 0 and 1.</param>
	/// <param name="rWeight">The red weight. The argument must be between 0 and 1.</param>
	/// <param name="gWeight">The green weight. The argument must be between 0 and 1.</param>
	/// <param name="bWeight">The blue weight. The argument must be between 0 and 1.</param>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when one argument of <paramref name="aWeight"/>, <paramref name="rWeight"/>,
	/// <paramref name="gWeight"/> and <paramref name="bWeight"/> isn't between 0 and 1.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UIColor(double aWeight, double rWeight, double gWeight, double bWeight)
	: this(
		ArgbArgCheck(aWeight, nameof(aWeight)),
		ArgbArgCheck(rWeight, nameof(rWeight)),
		ArgbArgCheck(gWeight, nameof(gWeight)),
		ArgbArgCheck(bWeight, nameof(bWeight))
	)
	{
	}


	/// <summary>
	/// Indicates the mask.
	/// </summary>
	private int Mask
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => A << 24 | R << 16 | G << 8 | B;
	}

	/// <summary>
	/// Indicates the <see cref="UIColor"/> instance only contains the R, G, B values.
	/// </summary>
	private UIColor RgbColorInstance
	{
		[LambdaBody]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => new(R, G, B);
	}

	/// <inheritdoc/>
	static UIColor IMinMaxValue<UIColor>.MinValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => MinValue;
	}

	/// <inheritdoc/>
	static UIColor IMinMaxValue<UIColor>.MaxValue
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => MaxValue;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(UIColor other) => Mask == other.Mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => Mask;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(null, null);

	/// <summary>
	/// Formats the value of the current instance using the specified format.
	/// </summary>
	/// <param name="format">The format.</param>
	/// <returns>The value of the current instance in the specified format.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format) => ToString(format, null);

	/// <inheritdoc/>
	/// <exception cref="FormatException">Throws when the <paramref name="format"/> is invalid.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format, IFormatProvider? formatProvider) =>
		formatProvider.HasFormatted(this, format, out string? result) ? result : format switch
		{
			null or "D" or "d" or "#" => Mask.ToString(), // Decimal type.
			"F" or "f" or ":" => $"A: {A}, R: {R}, G: {G}, B: {B}", // Full format type.
			_ => throw new FormatException("The specified format is invalid.")
		};

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	bool IValueEquatable<UIColor>.Equals(in UIColor other) => Equals(other);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UIColor Parse(string? str)
	{
		Nullability.ThrowIfNull(str);

		return parseArgb(str, out var resultArgb)
			? resultArgb
			: parseBracket(str, out var resultBracket)
				? resultBracket
				: throw new FormatException("The specified string is invalid to parse.");


		static bool parseArgb(string str, [DiscardWhen(false)] out UIColor result)
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

		static bool parseBracket(string str, [DiscardWhen(false)] out UIColor result)
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
	public static bool TryParse([NotNullWhen(true)] string? str, [DiscardWhen(false)] out UIColor result)
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
	/// The argument checking method that is only used in constructor
	/// <see cref="UIColor(double, double, double)"/> and <see cref="UIColor(double, double, double, double)"/>
	/// to check whether the specified value
	/// is between 0 and 1. If so, return the converted value; otherwise,
	/// an <see cref="ArgumentOutOfRangeException"/> instance will be thrown.
	/// </summary>
	/// <param name="arg">The argument to check.</param>
	/// <param name="argName">
	/// <para>Indicates the argument name.</para>
	/// <para>
	/// This argument will be automatically generated the value,
	/// so you don't need to re-assign a new value, because this argument is marked
	/// <c>[CallerArgumentExpression("arg")]</c>.
	/// </para>
	/// </param>
	/// <returns>The result converted.</returns>
	/// <exception cref="ArgumentOutOfRangeException">Throws when argument is invalid.</exception>
	/// <seealso cref="UIColor(double, double, double)"/>
	/// <seealso cref="UIColor(double, double, double, double)"/>
	[MethodImpl(MethodImplOptions.NoInlining)]
	private static byte ArgbArgCheck(double arg, [CallerArgumentExpression("arg")] string? argName = null) =>
		(byte)(byte.MaxValue * (arg is >= 0 and <= 1 ? arg : throw new ArgumentOutOfRangeException(argName)));


	/// <summary>
	/// Reverses a <see cref="UIColor"/>, that converts the A, R, G, B values of the base color
	/// to the reverse value, which is equivalent to <c>255 - value</c>.
	/// </summary>
	/// <param name="baseColor">The base color.</param>
	/// <returns>The <see cref="UIColor"/> result.</returns>
	/// <remarks>
	/// Please note that the operator <b>doesn't</b> negate the alpha value,
	/// i.e. the value of the property <see cref="A"/>.
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static UIColor operator ~(UIColor baseColor) =>
		new(
			baseColor.A,
			(byte)(byte.MaxValue - baseColor.R),
			(byte)(byte.MaxValue - baseColor.G),
			(byte)(byte.MaxValue - baseColor.B)
		);


	/// <summary>
	/// Implicit cast from <see cref="UIColor"/> to <see cref="Color"/>.
	/// </summary>
	/// <param name="color">The <see cref="UIColor"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator Color(UIColor color) => Color.FromArgb(color.A, color.R, color.G, color.B);

	/// <summary>
	/// Implicit cast from <see cref="Color"/> to <see cref="UIColor"/>.
	/// </summary>
	/// <param name="color">The <see cref="Color"/> instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator UIColor(Color color) => new(color.A, color.R, color.G, color.B);
}
