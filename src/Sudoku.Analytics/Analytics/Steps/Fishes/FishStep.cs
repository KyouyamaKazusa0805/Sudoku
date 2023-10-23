using System.Diagnostics.CodeAnalysis;
using System.SourceGeneration;
using Sudoku.Analytics.Configuration;
using Sudoku.Concepts.Converters;
using Sudoku.Concepts.Parsers;
using Sudoku.Concepts.Primitive;
using Sudoku.Rendering;
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
) : Step(conclusions, views, options), ICoordinateObject<FishStep>
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
	private protected string InternalNotation => ToString(Options.Converter);


	/// <inheritdoc/>
	public string ToString(CoordinateConverter converter)
	{
		switch (converter)
		{
			case RxCyConverter c:
			{
				// Special optimization.
				var baseSets = c.HouseConverter(BaseSetsMask);
				var coverSets = c.HouseConverter(CoverSetsMask);
				var exofins = this switch
				{
					NormalFishStep { Fins: var f and not [] } => $" f{c.CellConverter(in f)} ",
					ComplexFishStep { Exofins: var f and not [] } => $" f{c.CellConverter(in f)} ",
					_ => string.Empty
				};
				var endofins = this switch
				{
					ComplexFishStep { Endofins: var e and not [] } => $"ef{c.CellConverter(in e)}",
					_ => string.Empty
				};
				return $@"{c.DigitConverter((Mask)(1 << Digit))} {baseSets}\{coverSets}{exofins}{endofins}";
			}
			case var c:
			{
				var comma = GetString("Comma");
				var digitString = c.DigitConverter((Mask)(1 << Digit));
				var baseSets = c.HouseConverter(BaseSetsMask);
				var coverSets = c.HouseConverter(CoverSetsMask);
				var exofins = this switch
				{
					NormalFishStep { Fins: var f and not [] } => $"{comma}{string.Format(GetString("ExofinsAre")!, c.CellConverter(in f))}",
					ComplexFishStep { Exofins: var f and not [] } => $"{comma}{string.Format(GetString("ExofinsAre")!, c.CellConverter(in f))}",
					_ => string.Empty
				};
				var endofins = this switch
				{
					ComplexFishStep { Endofins: var e and not [] } => $"{comma}{string.Format(GetString("EndofinsAre")!, c.CellConverter(in e))}",
					_ => string.Empty
				};
				return $@"{c.DigitConverter((Mask)(1 << Digit))}{comma}{baseSets}\{coverSets}{exofins}{endofins}";
			}
		}
	}

	/// <inheritdoc/>
	[DoesNotReturn]
	static FishStep ICoordinateObject<FishStep>.ParseExact(string str, CoordinateParser parser)
		=> throw new NotSupportedException("This method does not supported.");
}
