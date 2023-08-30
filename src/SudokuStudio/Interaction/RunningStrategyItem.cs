namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a type that describes for a running strategy item.
/// </summary>
/// <param name="Title">Indicates the title.</param>
/// <param name="Value">Indicates the value.</param>
/// <param name="Updater">The updater that can assign new value to the target place.</param>
public record struct RunningStrategyItem(string Title, string Value, scoped in RunningStrategyUpdater Updater);
