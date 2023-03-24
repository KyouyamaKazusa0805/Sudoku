namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Hidden Single</b> or <b>Last Digit</b> (for special cases) technique.
/// </summary>
public sealed class HiddenSingleStep(Conclusion[] conclusions, View[]? views, int cell, int digit, int house, bool enableAndIsLastDigit) :
	SingleStep(conclusions, views, cell, digit)
{
	/// <summary>
	/// Indicates whether currently options enable "Last Digit" technqiue, and the current instance is a real Last Digit.
	/// </summary>
	public bool EnableAndIsLastDigit { get; } = enableAndIsLastDigit;

	/// <inheritdoc/>
	public override decimal BaseDifficulty => this switch { { EnableAndIsLastDigit: true } => 1.1M, { House: < 9 } => 1.2M, _ => 1.5M };

	/// <summary>
	/// Indicates the house where the current Hidden Single technique forms.
	/// </summary>
	public int House { get; } = house;

	/// <inheritdoc/>
	public override string Format => R[EnableAndIsLastDigit ? "TechniqueFormat_LastDigit" : "TechniqueFormat_HiddenSingle"]!;

	/// <inheritdoc/>
	public override Technique Code
		=> EnableAndIsLastDigit ? Technique.LastDigit : (Technique)((int)Technique.HiddenSingleBlock + (int)House.ToHouseType());

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?> FormatInterpolatedParts
		=> new Dictionary<string, string[]?>
		{
			{ "en", EnableAndIsLastDigit ? new[] { DigitStr } : new[] { HouseStr } },
			{ "zh", EnableAndIsLastDigit ? new[] { DigitStr } : new[] { HouseStr } }
		};

	private string DigitStr => (Digit + 1).ToString();

	private string HouseStr => HouseFormatter.Format(1 << House);
}
