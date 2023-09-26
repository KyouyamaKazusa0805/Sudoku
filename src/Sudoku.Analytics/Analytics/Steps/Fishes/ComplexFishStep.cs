using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Facts;
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
public sealed partial class ComplexFishStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit,
	HouseMask baseSetsMask,
	HouseMask coverSetsMask,
	[DataMember] scoped ref readonly CellMap exofins,
	[DataMember] scoped ref readonly CellMap endofins,
	[DataMember] bool isFranken,
	[DataMember] bool? isSashimi
) : FishStep(conclusions, views, options, digit, baseSetsMask, coverSetsMask), IEquatableStep<ComplexFishStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 3.2M;

	/// <inheritdoc/>
	public override unsafe Technique Code
	{
		get
		{
			// Creates a buffer to store the characters that isn't a space or a bar.
			var name = internalName();
			var buffer = stackalloc char[name.Length];
			var bufferLength = 0;
			fixed (char* p = name)
			{
				for (var ptr = p; *ptr != '\0'; ptr++)
				{
					if (*ptr is not ('-' or ' '))
					{
						buffer[bufferLength++] = *ptr;
					}
				}
			}

			// Parses via the buffer, and returns the result.
			return Enum.Parse<Technique>(new string(PointerOperations.Slice(buffer, 0, bufferLength)));


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			string internalName()
			{
				var finKindStr = finKind() is var finModifier and not FinKind.Normal ? $"{finModifier} " : null;
				var shapeKindStr = shapeKind() is var shapeModifier and not ShapeKind.Basic ? $"{shapeModifier} " : null;
				return $"{finKindStr}{shapeKindStr}{TechniqueFact.GetFishEnglishName(Size)}";
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			FinKind finKind() => IsSashimi switch { true => FinKind.Sashimi, false => FinKind.Finned, _ => FinKind.Normal };

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			ShapeKind shapeKind() => IsFranken ? ShapeKind.Franken : ShapeKind.Mutant;
		}
	}

	/// <inheritdoc/>
	public override ExtraDifficultyCase[] ExtraDifficultyCases
		=> [
			new(ExtraDifficultyCaseNames.Size, Size switch { 2 => 0, 3 => .6M, 4 => 2.0M, 5 => 3.3M, 6 => 4.5M, 7 => 5.6M, _ => 6.6M }),
			new(
				ExtraDifficultyCaseNames.Sashimi,
				IsSashimi switch
				{
					false => Size switch { 2 or 3 or 4 => .2M, 5 or 6 or 7 => .3M, _ => .4M },
					true => Size switch { 2 or 3 => .3M, 4 or 5 => .4M, 6 => .5M, 7 => .6M, _ => .7M },
					_ => 0
				}
			),
			new(
				ExtraDifficultyCaseNames.FishShape,
				IsFranken
					? Size switch { 2 => 0, 3 or 4 => 1.1M, 5 or 6 or 7 => 1.2M, _ => 1.3M }
					: Size switch { 2 => 0, 3 or 4 => 1.4M, 5 or 6 => 1.6M, 7 => 1.7M, _ => 2.0M }
			)
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

/// <summary>
/// Indicates a shape modifier that is used for a complex fish pattern.
/// </summary>
[Flags]
file enum ShapeKind
{
	/// <summary>
	/// Indicates the basic fish.
	/// </summary>
	Basic = 1,

	/// <summary>
	/// Indicates the franken fish.
	/// </summary>
	Franken = 1 << 1,

	/// <summary>
	/// Indicates the mutant fish.
	/// </summary>
	Mutant = 1 << 2
}

/// <summary>
/// Indicates a fin modifier that is used for a complex fish pattern.
/// </summary>
[Flags]
file enum FinKind
{
	/// <summary>
	/// Indicates the normal fish (i.e. no fins).
	/// </summary>
	Normal = 1,

	/// <summary>
	/// Indicates the finned fish
	/// (i.e. contains fins, but the fish may be regular when the fins are removed).
	/// </summary>
	Finned = 1 << 1,

	/// <summary>
	/// Indicates the sashimi fish
	/// (i.e. contains fins, and the fish may be degenerated to hidden singles when the fins are removed).
	/// </summary>
	Sashimi = 1 << 2,

	/// <summary>
	/// Indicates the Siamese fish (i.e. two fish share same base sets, with different cover sets).
	/// </summary>
	Siamese = 1 << 3
}
