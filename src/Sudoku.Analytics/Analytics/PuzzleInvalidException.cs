namespace Sudoku.Analytics;

/// <summary>
/// Represents an exception that will be thrown if an invalid case has been encountered during analyzing a sudoku puzzle.
/// </summary>
/// <param name="stepSearcherType">The type of the step searcher that throws the exception.</param>
/// <remarks>
/// This exception will be thrown as an unexpected behavior. For example, the puzzle is checked as a unique puzzle,
/// but a step searcher may contain a bug that causes the problem throwing this exception. Please check for property
/// <see cref="StepSearcherType"/> to learn more information.
/// </remarks>
/// <seealso cref="StepSearcherType"/>
public sealed partial class PuzzleInvalidException([RecordParameter] Type stepSearcherType) : Exception;
