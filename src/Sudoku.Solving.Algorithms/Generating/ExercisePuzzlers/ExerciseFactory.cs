namespace Sudoku.Generating.ExercisePuzzlers;

/// <summary>
/// Defines a type that creates exercises.
/// </summary>
public static class ExerciseFactory
{
	/// <summary>
	/// Creates a full house puzzle.
	/// </summary>
	/// <returns>The puzzle.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static HouseCellChunk FullHouse() => FullHouseExerciseGenerator.Generate();
}
