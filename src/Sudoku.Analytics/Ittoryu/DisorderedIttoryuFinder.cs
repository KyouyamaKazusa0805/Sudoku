namespace Sudoku.Ittoryu;

/// <summary>
/// Represents a disordered ittoryu path finder. This finder will find a digit sequence order that makes the puzzle be an ittoryu.
/// This finder only uses single techniques (Hidden Singles and Naked Singles) to solve a puzzle;
/// complex singles won't be supported for now.
/// </summary>
/// <param name="supportedTechniques">Indicates the supported techniques. By default, all singles are included.</param>
public sealed partial class DisorderedIttoryuFinder([PrimaryConstructorParameter] params TechniqueSet supportedTechniques)
{
	/// <summary>
	/// Initializes a <see cref="DisorderedIttoryuFinder"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DisorderedIttoryuFinder() : this(TechniqueSets.IttoryuTechniques)
	{
	}

	/// <summary>
	/// Initialzes a <see cref="DisorderedIttoryuFinder"/> instance via the specified list of techniques.
	/// </summary>
	/// <param name="techniques">A list of techniques.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DisorderedIttoryuFinder(IEnumerable<Technique> techniques) : this([.. techniques])
	{
	}


	/// <summary>
	/// Find a suitable ittoryu path.
	/// </summary>
	/// <param name="grid">The grid to be checked.</param>
	/// <returns>
	/// The target digit path. If none found, a longest path will be returned.
	/// </returns>
	public DisorderedIttoryuDigitPath FindPath(ref readonly Grid grid)
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
			ReadOnlySpan<PathNode> foundNodes,
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

		void fullHouses(ref readonly Grid grid, List<PathNode> foundNodes, Digit digit)
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

		void hiddenSingles(ref readonly Grid grid, List<PathNode> foundNodes, Digit digit)
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
				if (!SupportedTechniques.Contains(houseCode))
				{
					continue;
				}

				if (Subview.ReduceCellByHouse(HousesMap[house] & candidatesMap[digit], house) is var mask && IsPow2((uint)mask))
				{
					foundNodes.Add(new(in grid, house, HousesCells[house][Log2((uint)mask)] * 9 + digit));
				}
			}
		}

		void nakedSingles(ref readonly Grid grid, List<PathNode> foundNodes, Digit digit)
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

/// <summary>
/// Represents for a path node in a whole solving path via ittoryu solving logic.
/// </summary>
/// <param name="Grid">Indicates the currently-used grid.</param>
/// <param name="House">Indicates the house. The value can be -1 when the represented node is for a naked single.</param>
/// <param name="Candidate">Indicates the target candidate.</param>
file sealed record PathNode(ref readonly Grid Grid, House House, Candidate Candidate) : IFormattable
{
	/// <summary>
	/// Indicates the target digit.
	/// </summary>
	public Digit Digit => Candidate % 9;

	/// <summary>
	/// Indicates the target cell.
	/// </summary>
	public Cell Cell => Candidate / 9;


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Grid grid, out House house, out Cell cell, out Digit digit)
		=> ((grid, house, _), cell, digit) = (this, Candidate / 9, Candidate % 9);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => ToString(GlobalizedConverter.InvariantCultureConverter);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(IFormatProvider? formatProvider)
		=> ToString(GlobalizedConverter.GetConverter(formatProvider as CultureInfo ?? CultureInfo.CurrentUICulture));

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(CoordinateConverter converter)
		=> House != -1
			? $"Full House / Hidden Single: {converter.CandidateConverter(Candidate)} in house {converter.HouseConverter(1 << House)}"
			: $"Naked Single: {converter.CandidateConverter(Candidate)}";

	/// <inheritdoc/>
	string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => ToString(formatProvider);
}
