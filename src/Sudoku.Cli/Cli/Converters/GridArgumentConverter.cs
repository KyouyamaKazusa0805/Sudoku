namespace Sudoku.Cli.Converters;

/// <summary>
/// Defines a converter that converts the argument into a <see cref="Grid"/> result.
/// </summary>
public sealed class GridArgumentConverter : IArgumentConverter<GridArgumentConverter, Grid>
{
	/// <summary>
	/// Indicates the error message displayed when the argument text are whitespaces only.
	/// </summary>
	private const string ErrorMessage_OnlyWhitespaces = "The target argument should not be an empty string or only contain whitespaces";

	/// <summary>
	/// Indicates the error message displayed when the specified argument text cannot be parsed as a valid <see cref="Grid"/> instance.
	/// </summary>
	private const string ErrorMessage_InvalidGridText = "The target argument must be a valid sudoku text string.";


	/// <inheritdoc/>
	public static Grid ConvertValue(ArgumentResult argumentResult)
	{
		var str = argumentResult.Tokens.First(static token => token.Type == TokenType.Argument).Value;
		if (string.IsNullOrWhiteSpace(str))
		{
			argumentResult.ErrorMessage = ErrorMessage_OnlyWhitespaces;
			return Grid.Undefined;
		}

		if (!Grid.TryParse(str, out var s))
		{
			argumentResult.ErrorMessage = ErrorMessage_InvalidGridText;
			return Grid.Undefined;
		}
		return s;
	}
}
