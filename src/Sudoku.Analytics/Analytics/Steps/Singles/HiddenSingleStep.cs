namespace Sudoku.Analytics.Steps;

/// <summary>
/// Represents a data structure that describes for a technique of <b>Hidden Single</b>.
/// </summary>
/// <param name="conclusions"><inheritdoc cref="Step.Conclusions" path="/summary"/></param>
/// <param name="views"><inheritdoc cref="Step.Views" path="/summary"/></param>
/// <param name="options"><inheritdoc cref="Step.Options" path="/summary"/></param>
/// <param name="cell"><inheritdoc cref="SingleStep.Cell" path="/summary"/></param>
/// <param name="digit"><inheritdoc cref="SingleStep.Digit" path="/summary"/></param>
/// <param name="house">The house to be displayed.</param>
/// <param name="enableAndIsLastDigit">
/// Indicates whether currently options enable "Last Digit" technique, and the current instance is a real Last Digit.
/// If the technique is not a Last Digit, the value must be <see langword="false"/>.
/// </param>
/// <param name="lasting"><inheritdoc cref="ILastingTrait.Lasting" path="/summary"/></param>
/// <param name="subtype"><inheritdoc cref="SingleStep.Subtype" path="/summary"/></param>
/// <param name="excluderInfo">
/// Indicates the excluder information. The value can be <see langword="null"/> if the target step is a Last Digit.
/// </param>
public partial class HiddenSingleStep(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	Cell cell,
	Digit digit,
	[PrimaryConstructorParameter] House house,
	[PrimaryConstructorParameter] bool enableAndIsLastDigit,
	[PrimaryConstructorParameter] Cell lasting,
	SingleSubtype subtype,
	[PrimaryConstructorParameter] ExcluderInfo? excluderInfo
) : SingleStep(conclusions, views, options, cell, digit, subtype), ILastingTrait
{
	/// <inheritdoc/>
	public sealed override int BaseDifficulty
		=> EnableAndIsLastDigit ? 11 : House < 9 ? Options.IsDirectMode ? 12 : 19 : Options.IsDirectMode ? 15 : 23;

	/// <inheritdoc/>
	public sealed override Technique Code
		=> (Options.IsDirectMode, EnableAndIsLastDigit) switch
		{
			(_, true) => Technique.LastDigit,
			(true, false) => (Technique)((int)Technique.CrosshatchingBlock + (int)House.ToHouseType()),
			_ => (Technique)((int)Technique.HiddenSingleBlock + (int)House.ToHouseType())
		};

	/// <inheritdoc/>
	public sealed override InterpolationArray Interpolations
		=> [
			new(SR.EnglishLanguage, EnableAndIsLastDigit ? [DigitStr] : [HouseStr]),
			new(SR.ChineseLanguage, EnableAndIsLastDigit ? [DigitStr] : [HouseStr])
		];

	private string DigitStr => Options.Converter.DigitConverter((Mask)(1 << Digit));

	private string HouseStr => Options.Converter.HouseConverter(1 << House);


	/// <inheritdoc/>
	public override string GetName(IFormatProvider? formatProvider)
	{
		var baseName = base.GetName(formatProvider);
		if (!Options.IsDirectMode || Code is Technique.LastDigit or Technique.CrosshatchingBlock)
		{
			return baseName;
		}

		var culture = Options.CurrentCulture;
		var lastDigitsCountString = string.Format(
			SR.Get("DirectSingleLastSuffix", culture),
			null, // Placeholder for the house type - hidden singles won't check on this case.
			TechniqueNaming.GetDigitCharacter(culture, Lasting - 1)
		);
		if (SR.IsChinese(culture))
		{
			var centerDot = SR.Get("_Token_CenterDot", culture);
			return $"{baseName}{centerDot}{lastDigitsCountString}";
		}
		return $"{baseName} ({lastDigitsCountString})";
	}

	/// <inheritdoc/>
	protected override int NameCompareTo(Step other, IFormatProvider? formatProvider)
	{
		var (leftCode, rightCode) = (d(Code), d(other.Code));
		if (leftCode.CompareTo(rightCode) is var codeComparisonResult and not 0)
		{
			return codeComparisonResult;
		}

		var culture = GetCulture(formatProvider);
		if (SR.IsChinese(culture))
		{
			return base.NameCompareTo(other, formatProvider);
		}

		var leftName = GetName(formatProvider);
		var rightName = other.GetName(formatProvider);
		var leftDigit = TechniqueNaming.GetChineseDigit(TechniqueNaming.ChineseDigitsPattern.Match(leftName).Value[0]);
		var rightDigit = TechniqueNaming.GetChineseDigit(TechniqueNaming.ChineseDigitsPattern.Match(rightName).Value[0]);
		return leftDigit.CompareTo(rightDigit);


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static Technique d(Technique @base)
			=> @base switch
			{
				Technique.CrosshatchingBlock or Technique.HiddenSingleBlock => Technique.HiddenSingleBlock,
				Technique.CrosshatchingRow or Technique.HiddenSingleRow => Technique.HiddenSingleRow,
				_ => Technique.HiddenSingleColumn
			};
	}
}
