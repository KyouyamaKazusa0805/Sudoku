using System.Algorithm;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using Sudoku.Analytics.Categorization;
using Sudoku.Analytics.Configuration;
using Sudoku.Analytics.Rating;
using Sudoku.Concepts;
using Sudoku.Rendering;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Unique Loop</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit1">Indicates the first digit used.</param>
/// <param name="digit2">Indicates the second digit used.</param>
/// <param name="loop">Indicates the whole loop of cells used.</param>
/// <param name="loopPath">Indicates the loop path.</param>
public abstract partial class UniqueLoopStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[Data] Digit digit1,
	[Data] Digit digit2,
	[Data] scoped ref readonly CellMap loop,
	[Data] Cell[] loopPath
) : DeadlyPatternStep(conclusions, views, options), IEquatableStep<UniqueLoopStep>
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => 4.5M;

	/// <summary>
	/// Indicates the type.
	/// </summary>
	public abstract int Type { get; }

	/// <inheritdoc/>
	public sealed override Technique Code => Enum.Parse<Technique>($"UniqueLoopType{Type}");

	/// <inheritdoc/>
	public override ExtraDifficultyFactor[] ExtraDifficultyFactors => [new(ExtraDifficultyFactorNames.Length, (Sequences.A004526(Loop.Count) - 3) * .1M)];

	private protected string LoopStr => Options.Converter.CellConverter(Loop);

	private protected string Digit1Str => Options.Converter.DigitConverter((Mask)(1 << Digit1));

	private protected string Digit2Str => Options.Converter.DigitConverter((Mask)(1 << Digit2));


	/// <summary>
	/// Try to get the loop path score.
	/// </summary>
	private protected (int HouseTypeScore, int HousePositionScore) GetLoopPathScore()
	{
		var result = (HouseTypeScore: 0, HousePositionScore: 0);
		for (var i = 0; i < LoopPath.Length - 1; i++)
		{
			var a = LoopPath[i];
			var b = LoopPath[i + 1];
			(CellMap.Empty + a + b).InOneHouse(out var house);

			result.HouseTypeScore += HotSpot.GetHotSpot(house);
			result.HousePositionScore += house.ToHouseType() switch { HouseType.Block => 1, HouseType.Row => 3, HouseType.Column => 6 };
		}

		return result;
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static bool IEquatableStep<UniqueLoopStep>.operator ==(UniqueLoopStep left, UniqueLoopStep right)
		=> (left.Type, left.Loop, left.Digit1, left.Digit2) == (right.Type, right.Loop, right.Digit1, right.Digit2)
		&& (left, right) switch
		{
			(UniqueLoopType3Step { SubsetDigitsMask: var a }, UniqueLoopType3Step { SubsetDigitsMask: var b }) => a == b,
			(UniqueLoopType4Step { ConjugatePair: var a }, UniqueLoopType4Step { ConjugatePair: var b }) => a == b,
			_ => true
		};
}
