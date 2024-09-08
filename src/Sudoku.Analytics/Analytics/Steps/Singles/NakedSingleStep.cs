namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for a technique of <b>Naked Single</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="Step.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="Step.Views" path="/summary"/></param>
/// <param name="options"><inheritdoc cref="Step.Options" path="/summary"/></param>
/// <param name="cell"><inheritdoc cref="SingleStep.Cell" path="/summary"/></param>
/// <param name="digit"><inheritdoc cref="SingleStep.Digit" path="/summary"/></param>
/// <param name="subtype"><inheritdoc cref="SingleStep.Subtype" path="/summary"/></param>
/// <param name="lasting"><inheritdoc cref="ILastingTrait.Lasting" path="/summary" /></param>
public sealed partial class NakedSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	Cell cell,
	Digit digit,
	SingleSubtype subtype,
	[PrimaryConstructorParameter] int lasting
) : SingleStep(conclusions, views, options, cell, digit, subtype), ILastingTrait
{
	/// <inheritdoc/>
	public override int BaseDifficulty => Options.IsDirectMode ? 23 : 10;

	/// <inheritdoc/>
	public override Technique Code => Technique.NakedSingle;


	/// <inheritdoc/>
	public override string GetName(IFormatProvider? formatProvider)
	{
		var baseName = base.GetName(formatProvider);
		if (!Options.IsDirectMode)
		{
			return baseName;
		}

		var culture = Options.CurrentCulture;
		var lastDigitsCountString = string.Format(
			SR.Get("LastPrefix", culture),
			TechniqueNaming.GetDigitCharacter(culture, Lasting - 1)
		);
		if (SR.IsChinese(culture))
		{
			var centerDot = SR.Get("_Token_CenterDot", culture);
			return $"{baseName}{centerDot}{lastDigitsCountString}";
		}
		return $"{baseName} ({lastDigitsCountString})";
	}
}
