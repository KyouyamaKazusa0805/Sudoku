namespace Sudoku.Concepts;

/// <summary>
/// Provides a module that checks for Siamese rules.
/// </summary>
internal static class Siamese
{
	/// <summary>
	/// The backing field that will be used for formatting notations, especially for conclusions,
	/// in order to check equality of two set of conclusions.
	/// </summary>
	private static readonly RxCyConverter NotationConverter = new();


	/// <summary>
	/// Get all possible Siamese XYZ-Rings for the specified accumulator.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <returns>All Siamese XYZ-Rings.</returns>
	public static ReadOnlySpan<XyzRingStep> GetSiamese(List<XyzRingStep> accumulator, scoped ref readonly Grid grid)
	{
		var result = new List<XyzRingStep>();
		scoped var stepsSpan = accumulator.AsReadOnlySpan();
		for (var index1 = 0; index1 < accumulator.Count - 1; index1++)
		{
			var xyz1 = stepsSpan[index1];
			for (var index2 = index1 + 1; index2 < accumulator.Count; index2++)
			{
				var xyz2 = stepsSpan[index2];
				if (check(in grid, xyz1, xyz2, out var siameseStep))
				{
					result.Add(siameseStep);
				}
			}
		}
		return result.AsReadOnlySpan();


		static bool check(scoped ref readonly Grid grid, XyzRingStep xyz1, XyzRingStep xyz2, [NotNullWhen(true)] out XyzRingStep? siameseStep)
		{
			if ((xyz1.Pivot, xyz1.LeafCell1, xyz1.LeafCell2) != (xyz2.Pivot, xyz2.LeafCell1, xyz2.LeafCell2))
			{
				// Should be same XYZ-Wing.
				goto ReturnFalse;
			}

			if (xyz1.IntersectedDigit != xyz2.IntersectedDigit)
			{
				// They should contain a same digit Z in XYZ-Wing pattern.
				goto ReturnFalse;
			}

			var house1 = Log2((uint)xyz1.ConjugateHousesMask);
			var house2 = Log2((uint)xyz2.ConjugateHousesMask);
			if ((HousesMap[house1] & CandidatesMap[xyz1.IntersectedDigit]) == (HousesMap[house2] & CandidatesMap[xyz1.IntersectedDigit]))
			{
				// They cannot hold a same cells of the conjugate.
				goto ReturnFalse;
			}

			var mergedConclusions = (xyz1.Conclusions.AsConclusionSet() | xyz2.Conclusions.AsConclusionSet()).ToArray();
			var xyz1ViewNodes = xyz1.Views![0];
			var xyz2ViewNodes = xyz2.Views![0];
			siameseStep = new(
				mergedConclusions,
				[
					[
						.. from node in xyz1ViewNodes where node.Identifier == ColorIdentifier.Auxiliary2 select node,
						.. from node in xyz2ViewNodes where node.Identifier == ColorIdentifier.Auxiliary2 select node,
						.. xyz1ViewNodes
					],
					xyz1ViewNodes,
					xyz2ViewNodes
				],
				xyz1.Options,
				xyz1.IntersectedDigit,
				xyz1.Pivot,
				xyz1.LeafCell1,
				xyz1.LeafCell2,
				xyz1.ConjugateHousesMask | xyz2.ConjugateHousesMask,
				xyz1.IsNice && xyz2.IsNice,
				xyz1.IsGrouped || xyz2.IsGrouped,
				true
			);
			return true;

		ReturnFalse:
			siameseStep = null;
			return false;
		}
	}

	/// <summary>
	/// Get all possible Siamese fishes for the specified accumulator.
	/// </summary>
	/// <typeparam name="T">The type of the fish.</typeparam>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <returns>All Siamese fish.</returns>
	public static ReadOnlySpan<T> GetSiamese<T>(List<T> accumulator, scoped ref readonly Grid grid) where T : FishStep
	{
		var result = new List<T>();
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


		static bool check(scoped ref readonly Grid puzzle, T fish1, T fish2, [NotNullWhen(true)] out T? siameseStep)
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

			siameseStep = (T)(Step)(
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
						mergedFins & (p.Exofins | ((ComplexFishStep)(Step)fish2).Exofins),
						mergedFins & (p.Endofins | ((ComplexFishStep)(Step)fish2).Endofins),
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
}
