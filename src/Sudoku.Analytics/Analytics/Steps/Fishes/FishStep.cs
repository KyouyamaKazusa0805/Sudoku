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
/// <param name="fins">Indicates the fins used.</param>
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
/// <param name="isSiamese">Indicates whether the pattern is a Siamese Fish.</param>
public abstract partial class FishStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	[PrimaryConstructorParameter] Digit digit,
	[PrimaryConstructorParameter] HouseMask baseSetsMask,
	[PrimaryConstructorParameter] HouseMask coverSetsMask,
	[PrimaryConstructorParameter] scoped ref readonly CellMap fins,
	[PrimaryConstructorParameter] bool? isSashimi,
	[PrimaryConstructorParameter] bool isSiamese = false
) : Step(conclusions, views, options), ICoordinateObject<FishStep>, ISizeTrait
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

	/// <inheritdoc/>
	public int Rank => 0;

	/// <summary>
	/// The internal notation.
	/// </summary>
	private protected string InternalNotation => ToString(Options.Converter);


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public new string ToString(CultureInfo? culture = null)
		=> ToString(culture is null ? GlobalizedConverter.InvariantCultureConverter : GlobalizedConverter.GetConverter(culture));

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
				var exofinsAre = ResourceDictionary.Get("ExofinsAre", ResultCurrentCulture);
				var comma = ResourceDictionary.Get("Comma", ResultCurrentCulture);
				var digitString = c.DigitConverter((Mask)(1 << Digit));
				var baseSets = c.HouseConverter(BaseSetsMask);
				var coverSets = c.HouseConverter(CoverSetsMask);
				var exofins = this switch
				{
					NormalFishStep { Fins: var f and not [] } => $"{comma}{string.Format(exofinsAre, c.CellConverter(in f))}",
					ComplexFishStep { Exofins: var f and not [] } => $"{comma}{string.Format(exofinsAre, c.CellConverter(in f))}",
					_ => string.Empty
				};
				var endofins = this switch
				{
					ComplexFishStep { Endofins: var e and not [] } => $"{comma}{string.Format(exofinsAre, c.CellConverter(in e))}",
					_ => string.Empty
				};
				return $@"{c.DigitConverter((Mask)(1 << Digit))}{comma}{baseSets}\{coverSets}{exofins}{endofins}";
			}
		}
	}


	/// <inheritdoc/>
	[DoesNotReturn]
	static FishStep ICoordinateObject<FishStep>.ParseExact(string str, CoordinateParser parser)
		=> throw new NotSupportedException(ResourceDictionary.ExceptionMessage("MemberNotSupported"));
}
