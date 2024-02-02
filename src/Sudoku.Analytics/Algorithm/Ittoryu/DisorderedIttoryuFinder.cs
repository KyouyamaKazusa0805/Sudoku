namespace Sudoku.Algorithm.Ittoryu;

/// <summary>
/// Represents an ittoryu path finder. This finder will find a digit sequence order that makes the puzzle be an ittoryu.
/// This finder uses single techniques (Hidden Singles and Naked Singles) to solve a puzzle.
/// </summary>
/// <param name="supportedTechniques">Indicates the supported techniques. By default, all singles are included.</param>
public sealed partial class DisorderedIttoryuFinder([PrimaryConstructorParameter] TechniqueSet supportedTechniques)
{
	/// <summary>
	/// Initializes an <see cref="DisorderedIttoryuFinder"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DisorderedIttoryuFinder() : this(TechniqueSets.IttoryuTechniques)
	{
	}


	/// <summary>
	/// Find a suitable ittoryu path.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>
	/// The target digit path. If none found, a longest path will be returned.
	/// </returns>
	public DigitPath FindPath(scoped ref readonly Grid grid)
	{
		var digitsStack = new Stack<Digit>();
		try
		{
			for (var digit = 0; digit < 9; digit++)
			{
				dfs(grid, digit, digitsStack, [], 0, true);
			}
		}
		catch (AlreadyFinishedException)
		{
			return [.. digitsStack.Reverse()];
		}

		return null;


		void dfs(
			Grid grid,
			Digit digit,
			Stack<Digit> digitsStack,
			scoped ReadOnlySpan<PathNode> foundNodes,
			Mask finishedDigits,
			bool skipApplying = false
		)
		{
			if (skipApplying)
			{
				goto StartCheckingWithoutApplying;
			}

			if (foundNodes.Length == 0)
			{
				// No available steps can be applied.
				return;
			}

			// Apply all digits for the currently-found nodes.
			foreach (var node in foundNodes)
			{
				if (grid.GetState(node.Cell) == CellState.Empty)
				{
					grid.SetDigit(node.Cell, node.Digit);
				}
			}

		StartCheckingWithoutApplying:
			if (grid.ValuesMap[digit].Count != 9)
			{
				// If the current digit is not completed, we should continue searching for this digit.
				var tempNodes = new List<PathNode>(16);
				fullHouses(in grid, tempNodes, digit);
				hiddenSingles(in grid, tempNodes, digit);
				nakedSingles(in grid, tempNodes, digit);

				dfs(grid, digit, digitsStack, [.. tempNodes], finishedDigits);
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
					// Just find one.
					throw new AlreadyFinishedException();
				}

				// If not, we should search for available path nodes agagin, and iterate on them.
				var tempNodes = new List<PathNode>(16);
				foreach (var anotherDigit in (Mask)(Grid.MaxCandidatesMask & (Mask)~finishedDigits))
				{
					fullHouses(in grid, tempNodes, anotherDigit);
					hiddenSingles(in grid, tempNodes, anotherDigit);
					nakedSingles(in grid, tempNodes, anotherDigit);
				}

				// Iterate on found path nodes.
				foreach (var anotherDigit in MaskOperations.Create(from node in tempNodes select node.Digit))
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

		void fullHouses(scoped ref readonly Grid grid, List<PathNode> foundNodes, Digit digit)
		{
			if (!SupportedTechniques.Contains(Technique.FullHouse))
			{
				return;
			}

			var emptyCells = grid.EmptyCells;
			for (var house = 0; house < 27; house++)
			{
				if ((emptyCells & HousesMap[house]) is [var fullHouseCell] && TrailingZeroCount(grid.GetCandidates(fullHouseCell)) == digit)
				{
					foundNodes.Add(new(in grid, house, fullHouseCell * 9 + digit));
				}
			}
		}

		void hiddenSingles(scoped ref readonly Grid grid, List<PathNode> foundNodes, Digit digit)
		{
			scoped var candidatesMap = grid.CandidatesMap;
			for (var house = 0; house < 27; house++)
			{
				var houseCode = house.ToHouseType() switch
				{
					HouseType.Block => Technique.HiddenSingleBlock,
					HouseType.Row => Technique.HiddenSingleRow,
					_ => Technique.HiddenSingleColumn
				};
				if (!SupportedTechniques.Contains(houseCode))
				{
					continue;
				}

				if ((HousesMap[house] & candidatesMap[digit]) / house is var mask && IsPow2((uint)mask))
				{
					foundNodes.Add(new(in grid, house, HouseCells[house][Log2((uint)mask)] * 9 + digit));
				}
			}
		}

		void nakedSingles(scoped ref readonly Grid grid, List<PathNode> foundNodes, Digit digit)
		{
			if (!SupportedTechniques.Contains(Technique.NakedSingle))
			{
				return;
			}

			foreach (var cell in grid.EmptyCells)
			{
				if (grid.GetCandidates(cell) == 1 << digit)
				{
					foundNodes.Add(new(in grid, -1, cell * 9 + digit));
				}
			}
		}
	}
}

/// <summary>
/// The internal exception type that reports "Already finished" information, breaking the recursion.
/// </summary>
file sealed class AlreadyFinishedException : Exception;
