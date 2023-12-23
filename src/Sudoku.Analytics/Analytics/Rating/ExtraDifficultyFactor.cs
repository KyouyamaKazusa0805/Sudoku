using Sudoku.Text;

namespace Sudoku.Analytics.Rating;

/// <summary>
/// Defines a pair of data that represents the extra difficulty rating for a technique step, limited by its name and the value.
/// </summary>
/// <param name="FactorName"><inheritdoc cref="IRatingDataProvider.FactorName" path="/summary"/></param>
/// <param name="Value"><inheritdoc cref="IRatingDataProvider.Value" path="/summary"/></param>
public readonly record struct ExtraDifficultyFactor(string FactorName, decimal Value) : ICultureFormattable, IRatingDataProvider
{
	/// <inheritdoc cref="object.ToString"/>
	public override string ToString() => ToString(null);

	/// <inheritdoc/>
	public string ToString(CultureInfo? culture)
		=> GetString($"{nameof(ExtraDifficultyFactorNames)}_{FactorName}", culture ?? CultureInfo.CurrentUICulture) ?? FactorName;
}
