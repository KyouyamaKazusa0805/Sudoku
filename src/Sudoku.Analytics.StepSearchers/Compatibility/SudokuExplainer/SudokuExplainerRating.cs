namespace Sudoku.Compatibility.SudokuExplainer;

/// <summary>
/// Defines a pair of <see cref="SudokuExplainerRatingRange"/> instances indicating the result of difficulty rating
/// for the specified technique defined in application Sudoku Explainer.
/// </summary>
/// <param name="Original">The original difficulty rating of the technique.</param>
/// <param name="Advanced">The advanced difficulty rating of the technique.</param>
public readonly record struct SudokuExplainerRating(SudokuExplainerRatingRange? Original, SudokuExplainerRatingRange? Advanced);
