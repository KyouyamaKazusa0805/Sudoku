namespace Sudoku.Generating.ExercisePuzzlers;

/// <summary>
/// Defines an exercise generator.
/// </summary>
internal interface IExerciseGenerator
{
	/// <summary>
	/// Try to generate a puzzle.
	/// </summary>
	/// <returns>The result grid.</returns>
	public static abstract HouseCellChunk Generate();
}
