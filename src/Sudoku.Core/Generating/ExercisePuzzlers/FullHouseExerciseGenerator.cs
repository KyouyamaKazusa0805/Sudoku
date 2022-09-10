namespace Sudoku.Generating.ExercisePuzzlers;

/// <summary>
/// Defines a full-house technique exercise generator.
/// </summary>
internal abstract class FullHouseExerciseGenerator : IExerciseGenerator
{
	/// <inheritdoc/>
	public static unsafe HouseCellChunk Generate()
	{
		var numbers = stackalloc[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };

		// Shuffle.
		var random = Random.Shared;
		for (var i = 0; i < 10; i++)
		{
			var rawValue = random.Next(0, 81);
			int i1 = rawValue / 9, i2 = rawValue % 9;
			PointerOperations.Swap(numbers + i1, numbers + i2);
		}

		var house = random.Next(0, 27);
		var emptyCellIndex = random.Next(0, 9);
		numbers[emptyCellIndex] = -1; // Remove the cell.

		return new(house, numbers);
	}
}
