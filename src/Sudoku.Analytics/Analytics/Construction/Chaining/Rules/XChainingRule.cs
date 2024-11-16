namespace Sudoku.Analytics.Construction.Chaining.Rules;

/// <summary>
/// Represents a chaining rule on X rule (i.e. <see cref="LinkType.SingleDigit"/>).
/// </summary>
/// <seealso cref="LinkType.SingleDigit"/>
public sealed class XChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void GetLinks(ref ChainingRuleLinkContext context)
	{
		if (context.GetLinkOption(LinkType.SingleDigit) == LinkOption.None)
		{
			return;
		}

		// VARIABLE_DECLARATION_BEGIN
		_ = context.Grid is { CandidatesMap: var __CandidatesMap };
		// VARIABLE_DECLARATION_END

		// Strong.
		for (var digit = 0; digit < 9; digit++)
		{
			for (var house = 0; house < 27; house++)
			{
				if ((HousesMap[house] & __CandidatesMap[digit]) is not { Count: not (0 or 1) } cellsInThisHouse)
				{
					continue;
				}

				var mask = cellsInThisHouse / house;
				if (Mask.PopCount(mask) != 2)
				{
					continue;
				}

				var pos1 = Mask.TrailingZeroCount(mask);
				var pos2 = mask.GetNextSet(pos1);
				var node1 = new Node((HousesCells[house][pos1] * 9 + digit).AsCandidateMap(), false);
				var node2 = new Node((HousesCells[house][pos2] * 9 + digit).AsCandidateMap(), true);
				context.StrongLinks.AddEntry(node1, node2);
			}
		}

		// Weak.
		for (var digit = 0; digit < 9; digit++)
		{
			for (var house = 0; house < 27; house++)
			{
				if ((HousesMap[house] & __CandidatesMap[digit]) is not { Count: not (0 or 1) } cellsInThisHouse)
				{
					continue;
				}

				var mask = cellsInThisHouse / house;
				foreach (var combinationPair in mask.GetAllSets().GetSubsets(2))
				{
					var node1 = new Node((HousesCells[house][combinationPair[0]] * 9 + digit).AsCandidateMap(), true);
					var node2 = new Node((HousesCells[house][combinationPair[1]] * 9 + digit).AsCandidateMap(), false);
					context.WeakLinks.AddEntry(node1, node2);
				}
			}
		}
	}

	/// <inheritdoc/>
	public override void CollectOnNodes(ref ChainingRuleNextOnNodeContext context)
	{
		var currentNode = context.CurrentNode;
		if (currentNode is not { Map: [var startCandidate], IsOn: false })
		{
			return;
		}

		ref readonly var grid = ref context.Grid;
		ref readonly var originalGrid = ref context.OriginalGrid;
		var candidatesMap = grid.CandidatesMap;
		var startCell = startCandidate / 9;
		var digit = startCandidate % 9;
		var resultNodes = new HashSet<Node>();
		var nodesSupposedOff = context.NodesSupposedOff;
		foreach (var houseType in HouseTypes)
		{
			if ((HousesMap[startCell.ToHouse(houseType)] & candidatesMap[digit]) - startCell is [var endCell])
			{
				var mapToCheck = HousesMap[endCell.ToHouse(houseType)] & (originalGrid.CandidatesMap[digit] & ~candidatesMap[digit]);
				resultNodes.Add(
					new(
						(endCell * 9 + digit).AsCandidateMap(),
						true,
						[
							..
							from cell in mapToCheck
							select nodesSupposedOff.First(n => n.Map is [var c] && c == cell * 9 + digit)
						]
					)
				);
			}
		}
		context.Nodes = resultNodes.ToArray();
	}

	/// <inheritdoc/>
	public override void CollectOffNodes(ref ChainingRuleNextOffNodeContext context)
	{
		var currentNode = context.CurrentNode;
		if (currentNode is not { Map: [var startCandidate], IsOn: true })
		{
			return;
		}

		ref readonly var grid = ref context.Grid;
		var candidatesMap = grid.CandidatesMap;
		var startCell = startCandidate / 9;
		var digit = startCandidate % 9;
		var resultNodes = new HashSet<Node>();
		foreach (var houseType in HouseTypes)
		{
			foreach (var endCell in (HousesMap[startCell.ToHouse(houseType)] & candidatesMap[digit]) - startCell)
			{
				resultNodes.Add(new((endCell * 9 + digit).AsCandidateMap(), false));
			}
		}
		context.Nodes = resultNodes.ToArray();
	}
}
