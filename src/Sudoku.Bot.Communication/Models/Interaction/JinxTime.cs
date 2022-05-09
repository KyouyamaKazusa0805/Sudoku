namespace Sudoku.Bot.Communication.Models.Interaction;

/// <summary>
/// Indicates the jinx time span.
/// </summary>
public sealed class JinxTimeSpan
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	public static readonly JinxTimeSpan Default = new() { JinxEndTimestamp = null, JinxSeconds = null };

	/// <summary>
	/// Indicates the timestamp pattern.
	/// </summary>
	private static readonly Regex TimeStampPattern = new(StringResource.Get("JinxTimestampPattern")!);

	/// <summary>
	/// Indicates the time-countdown pattern.
	/// </summary>
	private static readonly Regex CountdownPattern = new(StringResource.Get("JinxTimeCountdownPattern")!);

	/// <summary>
	/// The table of all possible date time units, and the representing number of seconds.
	/// </summary>
	private static readonly Dictionary<string, int> SecondsTable = new()
	{
		{ StringResource.Get("DateTimeUnit_Year")!, 60 * 60 * 24 * 365 },
		{ StringResource.Get("DateTimeUnit_Week1")!, 60 * 60 * 24 * 7 },
		{ StringResource.Get("DateTimeUnit_Week2")!, 60 * 60 * 24 * 7 },
		{ StringResource.Get("DateTimeUnit_Day1")!, 60 * 60 * 24 },
		{ StringResource.Get("DateTimeUnit_Day2")!, 60 * 60 * 24 },
		{ StringResource.Get("DateTimeUnit_Hour1")!, 60 * 60 },
		{ StringResource.Get("DateTimeUnit_Hour2")!, 60 * 60 },
		{ StringResource.Get("DateTimeUnit_Minute1")!, 60 },
		{ StringResource.Get("DateTimeUnit_Minute2")!, 60 }
	};


	/// <summary>
	/// Initializes a <see cref="JinxTimeSpan"/> instance via the default time span (60 seconds).
	/// </summary>
	public JinxTimeSpan() : this(60)
	{
	}

	/// <summary>
	/// Initializes a <see cref="JinxTimeSpan"/> instance via the specified seconds.
	/// </summary>
	/// <param name="seconds">The specified seconds you want to jinx someone.</param>
	public JinxTimeSpan(int seconds) => JinxSeconds = seconds.ToString();

	/// <summary>
	/// Initializes a <see cref="JinxTimeSpan"/> instance via the specified timestamp value, as string representation.
	/// </summary>
	/// <param name="timestamp">
	/// The timestamp value that defines the time span. The format is <c>"yyyy-MM-dd HH:mm:ss"</c>.
	/// For example: <c>"2077-01-01 08:00:00"</c>.
	/// </param>
	public JinxTimeSpan(string timestamp)
		=> JinxEndTimestamp = new DateTimeOffset(Convert.ToDateTime(timestamp)).ToUnixTimeSeconds().ToString();


	/// <summary>
	/// Indicates the time that ends the jinx. The value is absolute.
	/// </summary>
	[JsonPropertyName("mute_end_timestamp")]
	public string? JinxEndTimestamp { get; set; }

	/// <summary>
	/// Indicates the jinx time span. In seconds.
	/// </summary>
	[JsonPropertyName("mute_seconds")]
	public string? JinxSeconds { get; set; }


	/// <summary>
	/// Indicates the <see cref="JinxTimeSpan"/> instance with one-minute time span.
	/// </summary>
	[JsonIgnore]
	public static JinxTimeSpan OneMinute => FromTimeSpanString("1分钟");

	
	/// <summary>
	/// Creates a <see cref="JinxTimeSpan"/> instance via the specified time span, as the string representation.
	/// </summary>
	/// <param name="timeString">The time-span string.</param>
	/// <returns>The <see cref="JinxTimeSpan"/> instance.</returns>
	public static JinxTimeSpan FromTimeSpanString(string timeString)
	{
		if (string.IsNullOrEmpty(timeString))
		{
			goto ReturnDefault;
		}

		if (
#pragma warning disable IDE0055
			TimeStampPattern.Match(timeString) is
			{
				Success: true,
				Groups: [
					_,
					{ Value: var a },
					{ Value: var b },
					{ Value: var c },
					{ Value: var d },
					{ Value: var e },
					{ Value: var f }
				]
			}
#pragma warning restore IDE0055
		)
		{
			var converted = Convert.ToDateTime($"{a}-{b}-{c} {d}:{e}:{f}");
			return new() { JinxEndTimestamp = new DateTimeOffset(converted).ToUnixTimeSeconds().ToString() };
		}
		else if (
			CountdownPattern.Match(timeString) is
			{
				Success: true,
				Groups: [_, { Value: var timeRawValue }, { Value: var jinxTimeSpanUnit }]
			}
		)
		{
			int seconds = SecondsTable.TryGetValue(jinxTimeSpanUnit, out int s) ? s : 1;
			if (int.TryParse(timeRawValue, out int timeVal))
			{
				seconds *= timeVal;
			}

			return new() { JinxSeconds = seconds.ToString() };
		}

	ReturnDefault:
		return Default;
	}
}
