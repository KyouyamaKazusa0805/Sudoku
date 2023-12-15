using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;
using Sudoku.Algorithm.Solving;
using Sudoku.Concepts;

namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a generator that is based on pattern.
/// </summary>
/// <param name="seedPattern"><inheritdoc cref="Pattern" path="/summary"/></param>
[StructLayout(LayoutKind.Auto)]
[Equals]
[GetHashCode]
[ToString]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref partial struct PatternBasedPuzzleGenerator([Data(DataMemberKinds.Field, RefKind = "ref readonly")] ref readonly CellMap seedPattern)
{
	/// <summary>
	/// The internal solver.
	/// </summary>
	private readonly BitwiseSolver _solver = new();

	/// <summary>
	/// Indicates the test grid.
	/// </summary>
	private Grid _testGrid;


	/// <summary>
	/// Indicates the predefind pattern used.
	/// </summary>
	public readonly ref readonly CellMap Pattern => ref _seedPattern;


	/// <summary>
	/// Try to generate a puzzle using the specified pattern.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A valid <see cref="Grid"/> pattern that has a specified pattern, with specified digits should be filled in.</returns>
	/// <exception cref="NotImplementedException">Always throws.</exception>
	public readonly Grid Generate(CancellationToken cancellationToken = default)
	{
		var patternCellsSorted = OrderPatternCellsViaConnectionDegrees();

		throw new NotImplementedException("I'll implement later.");
	}

	/// <summary>
	/// Order the pattern cells via connection complexity.
	/// </summary>
	/// <returns>The cells ordered.</returns>
	private readonly int[] OrderPatternCellsViaConnectionDegrees()
	{
		var isOrdered = CellMap.Empty;
		var result = new int[_seedPattern.Count];
		for (var index = 0; index < _seedPattern.Count; index++)
		{
			var maxRating = 0;
			var best = -1;
			for (var i = 0; i < 81; i++)
			{
				if (!_seedPattern.Contains(i) || isOrdered.Contains(i))
				{
					continue;
				}

				var rating = 0;
				for (var j = 0; j < 81; j++)
				{
					if (!_seedPattern.Contains(j) || i == j)
					{
						continue;
					}

					if (i.ToHouseIndex(HouseType.Block) == j.ToHouseIndex(HouseType.Block)
						|| i.ToHouseIndex(HouseType.Row) == j.ToHouseIndex(HouseType.Row)
						|| i.ToHouseIndex(HouseType.Column) == j.ToHouseIndex(HouseType.Column))
					{
						rating += isOrdered.Contains(j) ? 10000 : 100;
					}

					if (!isOrdered.Contains(j) && (i.ToBandIndex() == j.ToBandIndex() || i.ToTowerIndex() == j.ToTowerIndex())
						&& i.ToHouseIndex(HouseType.Block) == j.ToHouseIndex(HouseType.Block))
					{
						rating++;
					}
				}

				if (maxRating < rating)
				{
					maxRating = rating;
					best = i;
				}
			}

			isOrdered.Add(best);
			result[index] = best;
		}

		return result;
	}
}
