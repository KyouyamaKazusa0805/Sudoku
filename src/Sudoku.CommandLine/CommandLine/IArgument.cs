namespace Sudoku.CommandLine;

/// <summary>
/// Represents an argument.
/// </summary>
/// <typeparam name="T">The type of the result parsed from the argument.</typeparam>
internal interface IArgument<out T> : IOptionOrArgument<T>;
