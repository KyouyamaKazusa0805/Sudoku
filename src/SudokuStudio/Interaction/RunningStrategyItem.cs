namespace SudokuStudio.Interaction;

/// <summary>
/// Represents a type that describes for a running strategy item.
/// </summary>
/// <param name="TitleKey">Indicates the title key in resource.</param>
/// <param name="Value">Indicates the value.</param>
/// <param name="Updater">The updater that can assign new value to the target place.</param>
public record struct RunningStrategyItem(string TitleKey, string Value, scoped in RunningStrategyUpdater Updater);
