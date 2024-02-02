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
	[PrimaryCosntructorParameter] Digit digit,
	[PrimaryCosntructorParameter] HouseMask baseSetsMask,
	[PrimaryCosntructorParameter] HouseMask coverSetsMask,
	[PrimaryCosntructorParameter] scoped ref readonly CellMap fins,
	[PrimaryCosntructorParameter] bool? isSashimi,
	[PrimaryCosntructorParameter] bool isSiamese = false
) : Step(conclusions, views, options), ICoordinateObject<FishStep>, ISiameseSupporter<FishStep>
{
	/// <summary>
	/// The backing field that will be used for formatting notations, especially for conclusions,
	/// in order to check equality of two set of conclusions.
	/// </summary>
	private static readonly RxCyConverter NotationConverter = new();


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
				var comma = ResourceDictionary.Get("Comma", ResultCurrentCulture);
				var digitString = c.DigitConverter((Mask)(1 << Digit));
				var baseSets = c.HouseConverter(BaseSetsMask);
				var coverSets = c.HouseConverter(CoverSetsMask);
				var exofins = this switch
				{
					NormalFishStep { Fins: var f and not [] }
						=> $"{comma}{string.Format(ResourceDictionary.Get("ExofinsAre", ResultCurrentCulture), c.CellConverter(in f))}",
					ComplexFishStep { Exofins: var f and not [] }
						=> $"{comma}{string.Format(ResourceDictionary.Get("ExofinsAre", ResultCurrentCulture), c.CellConverter(in f))}",
					_ => string.Empty
				};
				var endofins = this switch
				{
					ComplexFishStep { Endofins: var e and not [] }
						=> $"{comma}{string.Format(ResourceDictionary.Get("EndofinsAre", ResultCurrentCulture), c.CellConverter(in e))}",
					_ => string.Empty
				};
				return $@"{c.DigitConverter((Mask)(1 << Digit))}{comma}{baseSets}\{coverSets}{exofins}{endofins}";
			}
		}
	}


	/// <inheritdoc/>
	public static ReadOnlySpan<FishStep> GetSiamese(List<FishStep> accumulator, scoped ref readonly Grid grid)
	{
		var result = new List<FishStep>();
		scoped var stepsSpan = accumulator.AsReadOnlySpan();
		for (var index1 = 0; index1 < accumulator.Count - 1; index1++)
		{
			var fish1 = stepsSpan[index1];
			for (var index2 = index1 + 1; index2 < accumulator.Count; index2++)
			{
				var fish2 = stepsSpan[index2];
				if (check(in grid, fish1, fish2, out var siameseStep))
				{
					// Siamese fish contain more eliminations, we should insert them into the first place.
					result.Add(siameseStep);
				}
			}
		}
		return result.AsReadOnlySpan();


		static bool check(scoped ref readonly Grid puzzle, FishStep fish1, FishStep fish2, [NotNullWhen(true)] out FishStep? siameseStep)
		{
			if (fish1.BaseSetsMask != fish2.BaseSetsMask || fish1.Digit != fish2.Digit)
			{
				// A Siamese fish must hold a pair of fish containing same base sets, with a same digit.
				goto ReturnFalse;
			}

			if (fish1.Fins == fish2.Fins || (fish1.Fins & fish2.Fins) == fish2.Fins || (fish2.Fins & fish1.Fins) == fish1.Fins)
			{
				// They shouldn't be a same fish, or all fins from one fish belongs to the other fish. 
				goto ReturnFalse;
			}

			if (!fish1.Fins)
			{
				// A Siamese fish must contain at least one fin.
				goto ReturnFalse;
			}

			if ((fish1, fish2) is (ComplexFishStep a, ComplexFishStep b) && FishModule.GetShapeKind(a) != FishModule.GetShapeKind(b))
			{
				// They cannot hold different kind of shapes.
				goto ReturnFalse;
			}

			if (NotationConverter.ConclusionConverter(fish1.Conclusions) == NotationConverter.ConclusionConverter(fish2.Conclusions))
			{
				// Two fish cannot contain total same conclusions.
				goto ReturnFalse;
			}

			// They can form a Siamese fish.
			// Check for merged data.
			var mergedFins = fish1.Fins | fish2.Fins;
			var coveredSetsMask = fish1.CoverSetsMask | fish2.CoverSetsMask;
			var siameseCoverSetsMask = fish1.CoverSetsMask ^ fish2.CoverSetsMask;
			var conclusions = (fish1.Conclusions.AsConclusionSet() | fish2.Conclusions.AsConclusionSet()).ToArray();
			var isSashimi = (fish1.IsSashimi, fish2.IsSashimi) switch
			{
				(true, not null) or (not null, true) => true,
				(false, false) => false,
				(null, null) => default(bool?),
				_ => throw new InvalidOperationException("The Sashimi property is invalid.")
			};

			// Check for cannibalism.
			var isCannibalism = false;
			foreach (var house in fish1.BaseSetsMask)
			{
				var cells = HousesMap[house] & CandidatesMap[fish1.Digit];
				if (Array.Exists(conclusions, conclusion => cells.Contains(conclusion.Cell)))
				{
					isCannibalism = true;
					break;
				}
			}

			// Normal fish contains a direct view, which will not be useful here.
			var fish1ViewNodes = fish1.Views![0];
			var fish2ViewNodes = fish2.Views![0];
			var view = (View)([
				.. collectViewNodes(fish1ViewNodes, fish2ViewNodes),
				.. collectViewNodes(fish2ViewNodes, fish1ViewNodes),
				.. from house in fish1.BaseSetsMask select new HouseViewNode(ColorIdentifier.Normal, house),
				.. from house in siameseCoverSetsMask select new HouseViewNode(ColorIdentifier.Auxiliary3, house),
				..
				from house in coveredSetsMask & ~siameseCoverSetsMask
				select new HouseViewNode(ColorIdentifier.Auxiliary2, house)
			]);

			siameseStep = (FishStep)(
				fish1 switch
				{
					NormalFishStep => new NormalFishStep(
						conclusions,
						[view, fish1ViewNodes, fish2ViewNodes],
						fish1.Options,
						fish1.Digit,
						fish1.BaseSetsMask,
						coveredSetsMask,
						in mergedFins,
						isSashimi,
						true
					),
					ComplexFishStep p => new ComplexFishStep(
						conclusions,
						[view, fish1ViewNodes, fish2ViewNodes],
						fish1.Options,
						fish1.Digit,
						fish1.BaseSetsMask,
						coveredSetsMask,
						mergedFins & (p.Exofins | ((ComplexFishStep)fish2).Exofins),
						mergedFins & (p.Endofins | ((ComplexFishStep)fish2).Endofins),
						FishModule.GetShapeKind(p) switch
						{
							FishShapeKind.Franken => true,
							FishShapeKind.Mutant => false,
							_ => throw new InvalidOperationException("A complex fish cannot hold a normal shape kind.")
						},
						isSashimi,
						isCannibalism,
						true
					),
					_ => throw new NotSupportedException("The target type is not supported.")
				}
			);
			return true;

		ReturnFalse:
			siameseStep = null;
			return false;


			static ReadOnlySpan<CandidateViewNode> collectViewNodes(View fish1ViewNodes, View fish2ViewNodes)
			{
				var result = new List<CandidateViewNode>();
				foreach (var node1 in fish1ViewNodes)
				{
					if (node1 is not CandidateViewNode(WellKnownColorIdentifier id1, var candidate1))
					{
						continue;
					}

					var n = fish2ViewNodes.FirstOrDefault(node => node is CandidateViewNode(_, var candidate2) && candidate1 == candidate2);
					if (n?.Identifier is not WellKnownColorIdentifier id2)
					{
						throw new InvalidOperationException("The view in the second fish is invalid.");
					}

					result.Add(
						new CandidateViewNode(
							(id1, id2) switch
							{
								({ Kind: WellKnownColorIdentifierKind.Endofin }, _) => ColorIdentifier.Endofin,
								(_, { Kind: WellKnownColorIdentifierKind.Endofin }) => ColorIdentifier.Endofin,
								({ Kind: WellKnownColorIdentifierKind.Exofin }, _) => ColorIdentifier.Exofin,
								(_, { Kind: WellKnownColorIdentifierKind.Exofin }) => ColorIdentifier.Exofin,
								_ => WellKnownColorIdentifierKind.Normal
							},
							candidate1
						)
					);
				}

				return result.AsReadOnlySpan();
			}
		}
	}

	/// <inheritdoc/>
	[DoesNotReturn]
	static FishStep ICoordinateObject<FishStep>.ParseExact(string str, CoordinateParser parser)
		=> throw new NotSupportedException("This method does not supported.");
}
