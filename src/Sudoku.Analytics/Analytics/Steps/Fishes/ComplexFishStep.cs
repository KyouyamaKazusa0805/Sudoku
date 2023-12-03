using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Fish</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="baseSetsMask"><inheritdoc/></param>
/// <param name="coverSetsMask"><inheritdoc/></param>
/// <param name="exofins">
/// Indicates the fins, positioned outside the fish.
/// They will be defined if cover sets cannot be fully covered, with the number of cover sets being equal to the number of base sets.
/// </param>
/// <param name="endofins">
/// Indicates the fins, positioned inside the fish. Generally speaking, they will be defined if they are in multiple base sets.
/// </param>
/// <param name="isFranken">
/// Indicates whether the fish is a Franken fish. If <see langword="true"/>, a Franken fish; otherwise, a Mutant fish.
/// </param>
/// <param name="isSashimi">
/// <para>Indicates whether the fish is a Sashimi fish.</para>
/// <para>
/// All cases are as below:
/// <list type="table">
/// <item>
/// <term><see langword="true"/></term>
/// <description>The fish is a sashimi finned fish.</description>
/// </item>
/// <item>
/// <term><see langword="false"/></term>
/// <description>The fish is a normal finned fish.</description>
/// </item>
/// <item>
/// <term><see langword="null"/></term>
/// <description>The fish doesn't contain any fin.</description>
/// </item>
/// </list>
/// </para>
/// </param>
/// <param name="isCannibalism">Indicates whether the fish contains any cannibalism.</param>
public sealed partial class ComplexFishStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit,
	HouseMask baseSetsMask,
	HouseMask coverSetsMask,
	[Data] scoped ref readonly CellMap exofins,
	[Data] scoped ref readonly CellMap endofins,
	[Data] bool isFranken,
	[Data] bool? isSashimi,
	[Data] bool isCannibalism
) : FishStep(conclusions, views, options, digit, baseSetsMask, coverSetsMask), IEquatableStep<ComplexFishStep>
{
	/// <summary>
	/// The letter or digit characters.
	/// </summary>
	private const string LetterOrDigitCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";


	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.2M;

	/// <inheritdoc/>
	public override unsafe Technique Code
	{
		get
		{
			// Creates a buffer to store the characters that isn't a space or a bar.
			scoped var name = internalName();
			scoped var buffer = (stackalloc char[name.Length]);
			var bufferLength = 0;
			foreach (var ch in name)
			{
				if (ch is not ('-' or ' '))
				{
					buffer[bufferLength++] = ch;
				}
			}

			return Enum.Parse<Technique>(buffer[..bufferLength]);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			ReadOnlySpan<char> internalName()
			{
				var finKindStr = finKind() is var finModifier and not FishFinKind.Normal ? $"{finModifier} " : null;
				var shapeKindStr = shapeKind() is var shapeModifier and not FishShapeKind.Basic ? $"{shapeModifier} " : null;
				return $"{finKindStr}{shapeKindStr}{TechniqueMarshal.GetFishEnglishName(Size)}";
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			FishFinKind finKind() => IsSashimi switch { true => FishFinKind.Sashimi, false => FishFinKind.Finned, _ => FishFinKind.Normal };

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			FishShapeKind shapeKind() => IsFranken ? FishShapeKind.Franken : FishShapeKind.Mutant;
		}
	}

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors
		=> [
			new(ExtraDifficultyFactorNames.Size, Size switch { 2 => 0, 3 => .6M, 4 => 2.0M, 5 => 3.3M, 6 => 4.5M, 7 => 5.6M, _ => 6.6M }),
			new(
				ExtraDifficultyFactorNames.Sashimi,
				IsSashimi switch
				{
					false => Size switch { 2 or 3 or 4 => .2M, 5 or 6 or 7 => .3M, _ => .4M },
					true => Size switch { 2 or 3 => .3M, 4 or 5 => .4M, 6 => .5M, 7 => .6M, _ => .7M },
					_ => 0
				}
			),
			new(
				ExtraDifficultyFactorNames.FishShape,
				IsFranken
					? Size switch { 2 => 0, 3 or 4 => 1.1M, 5 or 6 or 7 => 1.2M, _ => 1.3M }
					: Size switch { 2 => 0, 3 or 4 => 1.4M, 5 or 6 => 1.6M, 7 => 1.7M, _ => 2.0M }
			),
			new(ExtraDifficultyFactorNames.Cannibalism, IsCannibalism ? .3M : 0)
		];

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [InternalNotation]), new(ChineseLanguage, [InternalNotation])];


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<ComplexFishStep>.operator ==(ComplexFishStep left, ComplexFishStep right)
		=> left.Digit == right.Digit
		&& left.BaseSetsMask == right.BaseSetsMask && left.CoverSetsMask == right.CoverSetsMask
		&& left.Exofins == right.Exofins && left.Endofins == right.Endofins;
}
