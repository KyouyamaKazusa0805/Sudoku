namespace Sudoku.Ittoryu;

/// <summary>
/// Represents an ittoryu searching engine. This engine will search for single techniques:
/// <list type="bullet">
/// <item>Full House</item>
/// <item>Last Digit</item>
/// <item>Hidden Single</item>
/// <item>Naked Single</item>
/// </list>
/// </summary>
public sealed class SearchingEngine
{
	/// <summary>
	/// Indicates whether the engine supports for naked singles.
	/// </summary>
	public bool AllowNakedSingles { get; set; }


	/// <summary>
	/// Find a suitable ittoryu path.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>The target digit path. If none found, <see langword="null"/> will be returned. No exceptions will be thrown.</returns>
	public DigitPath? FindPath(scoped in Grid grid)
	{
		var digitsStack = new Stack<Digit>();
		try
		{
			var foundNodes = new List<PathNode>();
			for (var digit = 0; digit < 9; digit++)
			{
				ForFullHouse(grid, foundNodes, digit);
				ForHiddenSingle(grid, foundNodes, digit);
				ForNakedSingle(grid, foundNodes, digit);
			}

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
			return new(digitsStack.Reverse().ToArray());
		}

		return null;


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
				var tempNodes = new List<PathNode>(16);
				ForFullHouse(grid, tempNodes, digit);
				ForHiddenSingle(grid, tempNodes, digit);
				ForNakedSingle(grid, tempNodes, digit);

				dfs(grid, digit, digitsStack, tempNodes.ToArray(), finishedDigits);
			}
			else
			{
				digitsStack.Push(digit);
				finishedDigits |= (Mask)(1 << digit);

				if (finishedDigits == Grid.MaxCandidatesMask)
				{
					throw new InvalidOperationException();
				}

				var tempNodes = new List<PathNode>(16);
				foreach (var anotherDigit in (Mask)(Grid.MaxCandidatesMask & ~finishedDigits))
				{
					ForFullHouse(grid, tempNodes, anotherDigit);
					ForHiddenSingle(grid, tempNodes, anotherDigit);
					ForNakedSingle(grid, tempNodes, anotherDigit);
				}

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

				digitsStack.Pop();
			}
		}
	}

	/// <summary>
	/// Get all full houses.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <param name="foundNodes">A list of found path nodes.</param>
	/// <param name="digit">Indicates the digit to be checked.</param>
	private void ForFullHouse(scoped in Grid grid, List<PathNode> foundNodes, Digit digit)
	{
		var emptyCells = grid.EmptyCells;
		for (var house = 0; house < 27; house++)
		{
			if ((emptyCells & HousesMap[house]) is [var fullHouseCell])
			{
				var currentDigit = TrailingZeroCount(grid[HousesMap[house] - fullHouseCell]);
				if (currentDigit == digit)
				{
					foundNodes.Add(new(grid, house, fullHouseCell * 9 + digit));
				}
			}
		}
	}

	/// <summary>
	/// Get all hidden singles.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="ForFullHouse(in Grid, List{PathNode}, Digit)" path="/param[@name='grid']"/></param>
	/// <param name="foundNodes"><inheritdoc cref="ForFullHouse(in Grid, List{PathNode}, Digit)" path="/param[@name='foundNodes']"/></param>
	/// <param name="digit"><inheritdoc cref="ForFullHouse(in Grid, List{PathNode}, Digit)" path="/param[@name='digit']"/></param>
	private void ForHiddenSingle(scoped in Grid grid, List<PathNode> foundNodes, Digit digit)
	{
		var candidatesMap = grid.CandidatesMap;
		for (var house = 0; house < 27; house++)
		{
			if ((HousesMap[house] & candidatesMap[digit]) / house is var mask && IsPow2((uint)mask))
			{
				foundNodes.Add(new(grid, house, HouseCells[house][Log2((uint)mask)] * 9 + digit));
			}
		}
	}


	/// <summary>
	/// Get all naked singles.
	/// </summary>
	/// <param name="grid"><inheritdoc cref="ForFullHouse(in Grid, List{PathNode}, Digit)" path="/param[@name='grid']"/></param>
	/// <param name="foundNodes"><inheritdoc cref="ForFullHouse(in Grid, List{PathNode}, Digit)" path="/param[@name='foundNodes']"/></param>
	/// <param name="digit"><inheritdoc cref="ForFullHouse(in Grid, List{PathNode}, Digit)" path="/param[@name='digit']"/></param>
	private void ForNakedSingle(scoped in Grid grid, List<PathNode> foundNodes, Digit digit)
	{
		if (AllowNakedSingles)
		{
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

/// <summary>
/// Represents LINQ methods used in this file.
/// </summary>
file static class Extensions
{
	/// <inheritdoc cref="Enumerable.Where{TSource}(IEnumerable{TSource}, Func{TSource, bool})"/>
	public static ReadOnlySpan<TSource> Where<TSource>(this List<TSource> source, Func<TSource, bool> condition)
	{
		var result = new List<TSource>(source.Count);
		foreach (var element in source)
		{
			if (condition(element))
			{
				result.Add(element);
			}
		}

		return result.ToArray();
	}

	/// <inheritdoc cref="Enumerable.Select{TSource, TResult}(IEnumerable{TSource}, Func{TSource, TResult})"/>
	public static ReadOnlySpan<TResult> Select<TSource, TResult>(this List<TSource> source, Func<TSource, TResult> selector)
	{
		var result = new TResult[source.Count];
		var i = 0;
		foreach (var element in source)
		{
			result[i++] = selector(element);
		}

		return result;
	}

	/// <inheritdoc cref="Enumerable.Reverse{TSource}(IEnumerable{TSource})"/>
	public static Stack<T> Reverse<T>(this Stack<T> source)
	{
		var result = new Stack<T>(source.Count);
		foreach (var element in source)
		{
			result.Push(element);
		}

		return result;
	}
}
