namespace Sudoku.UI.DataConversion.ValueConverters;

/// <summary>
/// Defines a value converter that converts a grid into its string representation.
/// </summary>
public sealed class GridStringConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object? value, Type targetType, object? parameter, string? language)
	{
		return value switch
		{
			Grid { IsValid: false } => R["SudokuPage_InvalidGrid"]!,
			Grid { IsValid: true } grid => parameter switch
			{
				null => grid.ToString(),
				string format => grid.ToString(format),
				_ => @throw(nameof(parameter))
			},
			_ => @throw(nameof(value))
		};


		[DoesNotReturn]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string @throw(string argumentName)
			=> throw new ArgumentException($"The argument '{argumentName}' is invalid.", argumentName);
	}

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws due to not implemented.</exception>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language)
		=> throw new NotImplementedException();
}
