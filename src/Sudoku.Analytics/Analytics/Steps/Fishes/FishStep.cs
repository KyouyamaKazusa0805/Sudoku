using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Rendering;
using Sudoku.Text.Coordinate;
using static System.Numerics.BitOperations;
using static Sudoku.Analytics.Strings.StringsAccessor;

namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Fish</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit">Indicates the digit used.</param>
/// <param name="baseSetsMask">Indicates the mask that contains the base sets.</param>
/// <param name="coverSetsMask">Indicates the mask that contains the cover sets.</param>
public abstract partial class FishStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[DataMember] Digit digit,
	[DataMember] HouseMask baseSetsMask,
	[DataMember] HouseMask coverSetsMask
) : Step(conclusions, views, options), ICoordinateObject
{
	/// <inheritdoc/>
	/// <remarks>
	/// The name of the corresponding names are:
	/// <list type="table">
	/// <item><term>2</term><description>X-Wing</description></item>
	/// <item><term>3</term><description>Swordfish</description></item>
	/// <item><term>4</term><description>Jellyfish</description></item>
	/// <item><term>5</term><description>Squirmbag (or Starfish)</description></item>
	/// <item><term>6</term><description>Whale</description></item>
	/// <item><term>7</term><description>Leviathan</description></item>
	/// </list>
	/// Other fishes of sizes not appearing in above don't have well-known names.
	/// </remarks>
	public int Size => PopCount((uint)BaseSetsMask);

	/// <summary>
	/// The internal notation.
	/// </summary>
	private protected string InternalNotation => ToString(Options.CoordinateConverter);


	/// <inheritdoc/>
	public string ToString(CoordinateConverter coordinateConverter)
	{
		switch (coordinateConverter)
		{
			case RxCyConverter converter:
			{
				// Special optimization.
				var baseSets = converter.HouseNotationConverter(BaseSetsMask);
				var coverSets = converter.HouseNotationConverter(CoverSetsMask);
				var exofins = this switch
				{
					NormalFishStep { Fins: var f and not [] } => $" f{converter.CellNotationConverter(in f)} ",
					ComplexFishStep { Exofins: var f and not [] } => $" f{converter.CellNotationConverter(in f)} ",
					_ => string.Empty
				};
				var endofins = this switch
				{
					ComplexFishStep { Endofins: var e and not [] } => $"ef{converter.CellNotationConverter(in e)}",
					_ => string.Empty
				};
				return $@"{converter.DigitNotationConverter((Mask)(1 << Digit))} {baseSets}\{coverSets}{exofins}{endofins}";
			}
			case var converter:
			{
				var comma = GetString("Comma");
				var digitString = converter.DigitNotationConverter((Mask)(1 << Digit));
				var baseSets = converter.HouseNotationConverter(BaseSetsMask);
				var coverSets = converter.HouseNotationConverter(CoverSetsMask);
				var exofins = this switch
				{
					NormalFishStep { Fins: var f and not [] }
						=> $"{comma}{string.Format(GetString("ExofinsAre")!, converter.CellNotationConverter(in f))}",
					ComplexFishStep { Exofins: var f and not [] }
						=> $"{comma}{string.Format(GetString("ExofinsAre")!, converter.CellNotationConverter(in f))}",
					_ => string.Empty
				};
				var endofins = this switch
				{
					ComplexFishStep { Endofins: var e and not [] }
						=> $"{comma}{string.Format(GetString("EndofinsAre")!, converter.CellNotationConverter(in e))}",
					_ => string.Empty
				};
				return $@"{converter.DigitNotationConverter((Mask)(1 << Digit))}{comma}{baseSets}\{coverSets}{exofins}{endofins}";
			}
		}
	}
}
