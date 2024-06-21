namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents a fish module.
/// </summary>
internal static class FishModule
{
	/// <summary>
	/// The backing field that will be used for formatting notations, especially for conclusions,
	/// in order to check equality of two set of conclusions.
	/// </summary>
	private static readonly RxCyConverter NotationConverter = new();


	/// <summary>
	/// Check whether the fish is sashimi.
	/// </summary>
	/// <param name="baseSets">The base sets.</param>
	/// <param name="fins">All fins.</param>
	/// <param name="digit">The digit.</param>
	/// <returns>
	/// <para>A <see cref="bool"/> value indicating that.</para>
	/// <para>
	/// <inheritdoc cref="FishStep.IsSashimi" path="/remarks"/>
	/// </para>
	/// </returns>
	public static bool? IsSashimi(House[] baseSets, ref readonly CellMap fins, Digit digit)
	{
		if (!fins)
		{
			return null;
		}

		var isSashimi = false;
		foreach (var baseSet in baseSets)
		{
			if ((HousesMap[baseSet] & ~fins & CandidatesMap[digit]).Count == 1)
			{
				isSashimi = true;
				break;
			}
		}
		return isSashimi;
	}

	/// <summary>
	/// Get all possible Siamese patterns of type <see cref="FishStep"/> for the specified accumulator.
	/// </summary>
	/// <param name="accumulator">The accumulator.</param>
	/// <param name="grid">The grid.</param>
	/// <returns>All Siamese steps of type <see cref="FishStep"/>.</returns>
	public static ReadOnlySpan<FishStep> GetSiamese(List<FishStep> accumulator, ref readonly Grid grid)
	{
		var result = new List<FishStep>();
		var stepsSpan = accumulator.AsReadOnlySpan();
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


		static bool check(ref readonly Grid puzzle, FishStep fish1, FishStep fish2, [NotNullWhen(true)] out FishStep? siameseStep)
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

			if ((fish1, fish2) is (ComplexFishStep { Pattern.ShapeKind: var a }, ComplexFishStep { Pattern.ShapeKind: var b }) && a != b)
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
			var conclusions = (fish1.Conclusions.AsSet() | fish2.Conclusions.AsSet()).ToArray();
			var isSashimi = (fish1.IsSashimi, fish2.IsSashimi) switch
			{
				(true, not null) or (not null, true) => true,
				(false, false) => false,
				(null, null) => default(bool?),
				_ => throw new InvalidOperationException(SR.ExceptionMessage("SashimiPropertyInvalid"))
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

			siameseStep = fish1 switch
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
					p.Pattern.ShapeKind switch
					{
						FishShapeKind.Franken => true,
						FishShapeKind.Mutant => false,
						_ => throw new InvalidOperationException(SR.ExceptionMessage("ComplexFishCannotBeNormal"))
					},
					isSashimi,
					isCannibalism,
					true
				),
				_ => throw new NotSupportedException(SR.ExceptionMessage("MemberNotSupported"))
			};
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
						throw new InvalidOperationException(SR.ExceptionMessage("NormalFishViewInvalid"));
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
