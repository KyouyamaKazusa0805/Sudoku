namespace Sudoku.Bot.Communication;

/// <summary>
/// Defines a <see langword="static class"/> type that holds the <see langword="static"/> operations on logging.
/// </summary>
public static class Logging
{
	/// <summary>
	/// Indicates the output queue.
	/// </summary>
	private static readonly ConcurrentQueue<LogEntry> LogQueue = new();


	/// <summary>
	/// Indicates whether the bot is now working.
	/// </summary>
	private static bool _isWorking = false;


	/// <summary>
	/// Indicates the formatter for the formatting on a date time.
	/// The default value is <c>"HH:mm:ss.f"</c>, where:
	/// <list type="table">
	/// <listheader>
	/// <term>Formatting character</term>
	/// <description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>h</term>
	/// <description>Introduces the hour.</description>
	/// </item>
	/// <item>
	/// <term>m</term>
	/// <description>
	/// Introduces the minute. Please note that <c>M</c> is for month, and <c>m</c> is for minute.
	/// </description>
	/// </item>
	/// <item>
	/// <term>s</term>
	/// <description>Introduces the second.</description>
	/// </item>
	/// <item>
	/// <term>f</term>
	/// <description>Introduces the millisecond, nanosecond and so on.</description>
	/// </item>
	/// </list>
	/// </summary>
	public static string TimeFormatter { get; set; } = "HH:mm:ss.f";

	/// <summary>
	/// Indicates which level the log is will be displayed.
	/// </summary>
	public static LogLevel LogLevel { get; set; } = LogLevel.Info;

	/// <summary>
	/// Indicates the timestamp.
	/// </summary>
	private static string TimeStamp => $"[{DateTime.Now.ToString(TimeFormatter)}]";


	/// <summary>
	/// Indicates the event that is triggered when a log is going to be output.
	/// </summary>
	public static event Action<LogEntry>? LogTo;


	/// <summary>
	/// To print the log.
	/// </summary>
	/// <param name="logItem">The item to log.</param>
	private static void Print(LogEntry logItem)
	{
		_ = Task.Run(handler);


		void handler()
		{
			LogQueue.Enqueue(logItem);
			if (_isWorking)
			{
				return;
			}

			_isWorking = true;
			while (LogQueue.TryDequeue(out var entry))
			{
				if (LogLevel <= entry.Level)
				{
					Console.ForegroundColor = ConsoleColor.White;
					Console.Write($"{entry.TimeStamp}[{entry.Level.ToString()[0]}]");
					Console.ForegroundColor = entry.Level switch
					{
						LogLevel.Debug => ConsoleColor.Gray,
						LogLevel.Info => ConsoleColor.DarkGreen,
						LogLevel.Warning => ConsoleColor.DarkYellow,
						LogLevel.Error => ConsoleColor.DarkRed,
						_ => ConsoleColor.Magenta,
					};

					Console.WriteLine(UnicodeEncodingDecoding.Decode(entry.Message));
					Console.ResetColor();

					LogTo?.Invoke(entry);
				}
			}

			_isWorking = false;
		}
	}

	/// <summary>
	/// To print the debug information.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Debug(string message) => Print(new(LogLevel.Debug, message, TimeStamp));

	/// <summary>
	/// To print the normal information.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Info(string message) => Print(new(LogLevel.Info, message, TimeStamp));

	/// <summary>
	/// To print the warning information.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Warn(string message) => Print(new(LogLevel.Warning, message, TimeStamp));

	/// <summary>
	/// To print the error information.
	/// </summary>
	/// <param name="message">The message.</param>
	public static void Error(string message) => Print(new(LogLevel.Error, message, TimeStamp));
}
