namespace Sudoku.Bot.Communication;

/// <summary>
/// The log data.
/// </summary>
/// <param name="Level">The report level of the log.</param>
/// <param name="Message">The message content.</param>
/// <param name="TimeStamp">The timestamp.</param>
public readonly record struct LogEntry(LogLevel Level, string Message, string TimeStamp);
