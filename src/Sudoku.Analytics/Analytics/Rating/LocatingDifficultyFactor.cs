namespace Sudoku.Analytics.Rating;

/// <summary>
/// Represents a factor that describes for a difficulty factor describing how difficult that users can locate a technique pattern.
/// </summary>
/// <param name="FactorName"><inheritdoc cref="IRatingDataProvider.FactorName" path="/summary"/></param>
/// <param name="Value"><inheritdoc cref="IRatingDataProvider.Value" path="/summary"/></param>
public readonly record struct LocatingDifficultyFactor(string FactorName, decimal Value) : IRatingDataProvider;
