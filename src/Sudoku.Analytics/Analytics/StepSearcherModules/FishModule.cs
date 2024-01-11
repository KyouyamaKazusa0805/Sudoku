namespace Sudoku.Analytics.StepSearcherModules;

/// <summary>
/// Represents a fish module.
/// </summary>
internal static class FishModule
{
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
	public static bool? IsSashimi(House[] baseSets, scoped ref readonly CellMap fins, Digit digit)
	{
		if (!fins)
		{
			return null;
		}

		var isSashimi = false;
		foreach (var baseSet in baseSets)
		{
			if ((HousesMap[baseSet] - fins & CandidatesMap[digit]).Count == 1)
			{
				isSashimi = true;
				break;
			}
		}

		return isSashimi;
	}

	/// <summary>
	/// Determine the fish kind of the shape.
	/// </summary>
	/// <param name="pattern">The fish pattern.</param>
	/// <returns>The shape kind.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FishShapeKind GetShapeKind(FishStep pattern)
	{
		return pattern switch
		{
			ComplexFishStep { BaseSetsMask: var baseSets, CoverSetsMask: var coverSets } => (k(baseSets), k(coverSets)) switch
			{
				(FishShapeKind.Mutant, _) or (_, FishShapeKind.Mutant) => FishShapeKind.Mutant,
				(FishShapeKind.Franken, _) or (_, FishShapeKind.Franken) => FishShapeKind.Franken,
				_ => FishShapeKind.Basic
			},
			_ => FishShapeKind.Basic
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static FishShapeKind k(HouseMask mask)
		{
			var blockMask = mask & Grid.MaxCandidatesMask;
			var rowMask = mask >> 9 & Grid.MaxCandidatesMask;
			var columnMask = mask >> 18 & Grid.MaxCandidatesMask;
			return rowMask * columnMask != 0
				? FishShapeKind.Mutant
				: (rowMask | columnMask) != 0 && blockMask != 0
					? FishShapeKind.Franken
					: FishShapeKind.Basic;
		}
	}

	/// <summary>
	/// Check whether two fishes can be merged into one Siamese fish. If so, return <see langword="true"/>
	/// and assign valid values to <see langword="out"/> parameters of this method.
	/// </summary>
	/// <typeparam name="T">The type of the step for fish pattern.</typeparam>
	/// <param name="puzzle">The puzzle.</param>
	/// <param name="fish1">The fish pattern 1.</param>
	/// <param name="fish2">The fish pattern 2.</param>
	/// <param name="siameseFishStep">The final step.</param>
	/// <returns>A <see cref="bool"/> result indicating whether two fish can be merged into one Siamese fish.</returns>
	public static bool CheckSiamese<T>(scoped ref readonly Grid puzzle, T fish1, T fish2, [NotNullWhen(true)] out T? siameseFishStep)
		where T : FishStep
	{
		if (fish1.BaseSetsMask != fish2.BaseSetsMask || fish1.Digit != fish2.Digit)
		{
			// A Siamese fish must hold a pair of fish containing same base sets, with a same digit.
			goto ReturnFalse;
		}

		if (fish1.Fins == fish2.Fins)
		{
			// They are same fish.
			goto ReturnFalse;
		}

		if (!fish1.Fins)
		{
			// A Siamese fish must contain at least one fin.
			goto ReturnFalse;
		}

		if ((fish1, fish2) is (ComplexFishStep a, ComplexFishStep b) && GetShapeKind(a) != GetShapeKind(b))
		{
			// They cannot hold different kind of shapes.
			goto ReturnFalse;
		}

		// They can form a Siamese fish.
		// Check for merged data.
		var mergedFins = fish1.Fins | fish2.Fins;
		var coveredSetsMask = fish1.CoverSetsMask | fish2.CoverSetsMask;
		var conclusions = ((HashSet<Conclusion>)[.. fish1.Conclusions, .. fish2.Conclusions]).ToArray();
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
		var view = new View();
		collectViewNodes(view, fish1ViewNodes, fish2ViewNodes);
		collectViewNodes(view, fish2ViewNodes, fish1ViewNodes);
		view.AddRange(from house in fish1.BaseSetsMask select new HouseViewNode(WellKnownColorIdentifier.Normal, house));
		view.AddRange(from house in coveredSetsMask select new HouseViewNode(WellKnownColorIdentifier.Auxiliary2, house));

		siameseFishStep = (T)(Step)(
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
					GetShapeKind(p) switch
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
		siameseFishStep = null;
		return false;


		static void collectViewNodes(View view, View fish1ViewNodes, View fish2ViewNodes)
		{
			foreach (var node1 in fish1ViewNodes)
			{
				if (node1 is not CandidateViewNode(WellKnownColorIdentifier id1, var candidate1))
				{
					continue;
				}

				var n = fish2ViewNodes.Find(node => node is CandidateViewNode(_, var candidate2) && candidate1 == candidate2);
				if (n?.Identifier is not WellKnownColorIdentifier id2)
				{
					throw new InvalidOperationException("The view in the second fish is invalid.");
				}

				view.Add(
					new CandidateViewNode(
						(id1, id2) switch
						{
							({ Kind: WellKnownColorIdentifierKind.Endofin }, _) => WellKnownColorIdentifier.Endofin,
							(_, { Kind: WellKnownColorIdentifierKind.Endofin }) => WellKnownColorIdentifier.Endofin,
							({ Kind: WellKnownColorIdentifierKind.Exofin }, _) => WellKnownColorIdentifier.Exofin,
							(_, { Kind: WellKnownColorIdentifierKind.Exofin }) => WellKnownColorIdentifier.Exofin,
							_ => WellKnownColorIdentifierKind.Normal
						},
						candidate1
					)
				);
			}
		}
	}
}
