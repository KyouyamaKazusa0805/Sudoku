namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is an <b>Normal Fish</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="baseSetsMask"><inheritdoc/></param>
/// <param name="coverSetsMask"><inheritdoc/></param>
/// <param name="fins">Indicates the fins.</param>
/// <param name="isSashimi"><inheritdoc/></param>
/// <param name="isSiamese">Indicates whether the pattern is a Siamese Fish.</param>
public sealed partial class NormalFishStep(
	Conclusion[] conclusions,
	View[]? views,
	StepGathererOptions options,
	Digit digit,
	HouseMask baseSetsMask,
	HouseMask coverSetsMask,
	ref readonly CellMap fins,
	bool? isSashimi,
	bool isSiamese = false
) : FishStep(conclusions, views, options, digit, baseSetsMask, coverSetsMask, in fins, isSashimi, isSiamese)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 32;

	/// <inheritdoc/>
	public override Technique Code
	{
		get
		{
			var buffer = (stackalloc char[InternalName.Length]);
			var i = 0;
			foreach (var ch in InternalName)
			{
				if (ch is not (' ' or '-'))
				{
					buffer[i++] = ch;
				}
			}

			return Enum.Parse<Technique>(buffer[..i]);
		}
	}

	/// <inheritdoc/>
	public override InterpolationArray Interpolations
		=> [new(SR.EnglishLanguage, [InternalNotation]), new(SR.ChineseLanguage, [InternalNotation])];

	/// <inheritdoc/>
	public override FactorArray Factors
		=> [
			Factor.Create(
				"Factor_NormalFishSizeFactor",
				[nameof(Size)],
				GetType(),
				static args => (int)args![0]! switch { 2 => 0, 3 => 6, 4 => 20 }
			),
			Factor.Create(
				"Factor_NormalFishIsSashimiFactor",
				[nameof(IsSashimi), nameof(Size)],
				GetType(),
				static args => (bool?)args![0]! switch { true => (int)args![1]! switch { 2 or 3 => 3, 4 => 4 }, false => 2, _ => 0 }
			)
		];

	/// <summary>
	/// Indicates the internal name.
	/// </summary>
	private string InternalName
	{
		get
		{
			var prefix = (IsSiamese, IsSashimi) switch
			{
				(true, true) => "Siamese Sashimi ",
				(true, false) => "Siamese Finned ",
				(_, true) => "Sashimi ",
				(_, false) => "Finned ",
				(false, null) => string.Empty,
				_ => throw new InvalidOperationException($"Siamese fish requires a non-null value for property '{nameof(IsSashimi)}'.")
			};
			return $"{prefix}{TechniqueNaming.GetFishEnglishName(Size)}";
		}
	}
}
