namespace Sudoku.Algorithm.Ittoryu;

/// <summary>
/// Represents an ittoryu path finder. This finder will find a digit sequence order that makes the puzzle be an ittoryu.
/// This finder uses single techniques (Hidden Singles and Naked Singles) to solve a puzzle.
/// </summary>
public sealed class IttoryuPathFinder
{
	/// <summary>
	/// Indicates the found possible digit sequences.
	/// </summary>
	private readonly List<Digit[]> _foundSequences = [];


	/// <summary>
	/// Indicates the supported techniques. By default, all singles are included.
	/// </summary>
	public Technique[] SupportedTechniques { get; init; } = [
		Technique.FullHouse,
		Technique.HiddenSingleBlock,
		Technique.HiddenSingleRow,
		Technique.HiddenSingleColumn,
		Technique.NakedSingle
	];


	/// <summary>
	/// Find a suitable ittoryu path.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>
	/// The target digit path. If none found, a longest path will be returned.
	/// </returns>
	public DigitPath FindPath(scoped in Grid grid)
	{
		var digitsStack = new Stack<Digit>();
		try
		{
			var foundNodes = new List<PathNode>();
			for (var digit = 0; digit < 9; digit++)
			{
				fullHouses(grid, foundNodes, digit);
				hiddenSingles(grid, foundNodes, digit);
				nakedSingles(grid, foundNodes, digit);
			}

			_foundSequences.Clear();
			foreach (var digit in MaskCreator.Create(from node in foundNodes select node.Digit))
			{
				dfs(
					grid,
					digit,
					digitsStack,
					from node in foundNodes where node.Digit == digit select node,
					0
				);
			}
		}
		catch (InvalidOperationException)
		{
			return digitsStack.Reverse().ToArray();
		}

		if (_foundSequences.Count == 0)
		{
			return null;
		}

		return (from sequence in _foundSequences orderby sequence.Length descending select sequence).First();


		void dfs(Grid grid, Digit digit, Stack<Digit> digitsStack, scoped ReadOnlySpan<PathNode> foundNodes, Mask finishedDigits)
		{
			if (foundNodes.Length == 0)
			{
				return;
			}

			if (digitsStack.Count == 9)
			{
				// Just find one.
				throw new InvalidOperationException();
			}

			// Apply all digits for the currently-found nodes.
			foreach (var node in foundNodes)
			{
				if (grid.GetState(node.Cell) == CellState.Empty)
				{
					grid.SetDigit(node.Cell, node.Digit);
				}
			}

			if (grid.ValuesMap[digit].Count != 9)
			{
				// If the current digit is not completed, we should continue searching for this digit.
				var tempNodes = new List<PathNode>(16);
				fullHouses(grid, tempNodes, digit);
				hiddenSingles(grid, tempNodes, digit);
				nakedSingles(grid, tempNodes, digit);

				dfs(grid, digit, digitsStack, tempNodes.ToArray(), finishedDigits);
			}
			else
			{
				// If the current digit is completed, we should continue searching for the next digit.
				digitsStack.Push(digit);
				finishedDigits |= (Mask)(1 << digit);

				// Here we should check the bit mask. If currently we have found the digits are finished,
				// the last works are not necessary, just throw an exception to escape here.
				if (finishedDigits == Grid.MaxCandidatesMask)
				{
					throw new InvalidOperationException();
				}

				// Add a new sequence.
				_foundSequences.Add(digitsStack.Reverse().ToArray());

				// If not, we should search for available path nodes agagin, and iterate on them.
				var tempNodes = new List<PathNode>(16);
				foreach (var anotherDigit in (Mask)(Grid.MaxCandidatesMask & ~finishedDigits))
				{
					fullHouses(grid, tempNodes, anotherDigit);
					hiddenSingles(grid, tempNodes, anotherDigit);
					nakedSingles(grid, tempNodes, anotherDigit);
				}

				// Iterate on found path nodes.
				foreach (var anotherDigit in MaskCreator.Create(from node in tempNodes select node.Digit))
				{
					dfs(
						grid,
						anotherDigit,
						digitsStack,
						from node in tempNodes where node.Digit == anotherDigit select node,
						finishedDigits
					);
				}

				// If all available found path nodes cannot make the path complete, pop it.
				digitsStack.Pop();
			}
		}

		void fullHouses(scoped in Grid grid, List<PathNode> foundNodes, Digit digit)
		{
			if (Array.IndexOf(SupportedTechniques, Technique.FullHouse) == -1)
			{
				return;
			}

			var emptyCells = grid.EmptyCells;
			for (var house = 0; house < 27; house++)
			{
				if ((emptyCells & HousesMap[house]) is [var fullHouseCell]
					&& TrailingZeroCount(grid[HousesMap[house] - fullHouseCell]) == digit)
				{
					foundNodes.Add(new(grid, house, fullHouseCell * 9 + digit));
				}
			}
		}

		void hiddenSingles(scoped in Grid grid, List<PathNode> foundNodes, Digit digit)
		{
			var candidatesMap = grid.CandidatesMap;
			for (var house = 0; house < 27; house++)
			{
				var houseCode = house.ToHouseType() switch
				{
					HouseType.Block => Technique.HiddenSingleBlock,
					HouseType.Row => Technique.HiddenSingleRow,
					_ => Technique.HiddenSingleColumn
				};
				if (Array.IndexOf(SupportedTechniques, houseCode) == -1)
				{
					continue;
				}

				if ((HousesMap[house] & candidatesMap[digit]) / house is var mask && IsPow2((uint)mask))
				{
					foundNodes.Add(new(grid, house, HouseCells[house][Log2((uint)mask)] * 9 + digit));
				}
			}
		}

		void nakedSingles(scoped in Grid grid, List<PathNode> foundNodes, Digit digit)
		{
			if (Array.IndexOf(SupportedTechniques, Technique.NakedSingle) == -1)
			{
				return;
			}

			foreach (var cell in grid.EmptyCells)
			{
				if (grid.GetCandidates(cell) == 1 << digit)
				{
					foundNodes.Add(new(grid, -1, cell * 9 + digit));
				}
			}
		}
	}
}
