namespace Sudoku.Analytics.Rating;

/// <summary>
/// Defines a pair of data that represents the extra difficulty rating for a technique step, limited by its name and the value.
/// </summary>
/// <param name="Name">
/// <completionlist cref="ExtraDifficultyCaseNames"/>
/// The name of extra difficulty rating item.
/// </param>
/// <param name="Value">The rating value.</param>
public readonly record struct ExtraDifficultyCase(string Name, decimal Value);
