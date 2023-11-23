using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Rating;

/// <summary>
/// Defines a pair of data that represents the extra difficulty rating for a technique step, limited by its name and the value.
/// </summary>
/// <param name="Name">The name of extra difficulty rating factor name.</param>
/// <param name="Value">The factor value.</param>
public readonly record struct ExtraDifficultyFactor(string Name, decimal Value)
{
	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => GetString($"{nameof(ExtraDifficultyFactorNames)}_{Name}") ?? Name;
}
