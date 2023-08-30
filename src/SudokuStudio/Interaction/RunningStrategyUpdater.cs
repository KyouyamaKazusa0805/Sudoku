namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a type that encapsulates a router that can route a value to the distination.
/// </summary>
/// <param name="Title">The title to be assigned. This value is used for disctincting with options set.</param>
/// <param name="ValueRouter">The value router that can assign the value to the distinction.</param>
public record struct RunningStrategyUpdater(string Title, Action<object?> ValueRouter);
