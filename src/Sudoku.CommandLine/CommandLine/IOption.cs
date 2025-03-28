namespace Sudoku.CommandLine;

/// <summary>
/// Represents an option.
/// </summary>
/// <typeparam name="T">The type of the result parsed from the option.</typeparam>
public interface IOption<out T> : IOptionOrArgument<T>;
