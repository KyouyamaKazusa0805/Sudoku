using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Data;
using Sudoku.Solving.Manual;

namespace Sudoku.UI.Data.ValueConverters;

/// <summary>
/// Defines a value converter that allows the one-way binding from the
/// <see cref="AnalysisResultRow.DifficultyLevel"/> to <see cref="TextBlock.Text"/> property value.
/// </summary>
/// <seealso cref="AnalysisResultRow.DifficultyLevel"/>
/// <seealso cref="TextBlock.Text"/>
public sealed class DifficultyLevelToTextConverter : IValueConverter
{
	/// <inheritdoc/>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="targetType"/> is not <see cref="string"/>.
	/// </exception>
	[return: NotNullIfNotNull("value")]
	public object? Convert(object? value, Type targetType, object? parameter, string language) =>
		targetType != typeof(string)
			? throw new ArgumentException($"The desired target type must be 'string'.", nameof(targetType))
			: value switch
			{
				DifficultyLevel d => d switch
				{
					DifficultyLevel.Easy => Get("SudokuPage_AnalysisResultColumn_Easy"),
					DifficultyLevel.Moderate => Get("SudokuPage_AnalysisResultColumn_Moderate"),
					DifficultyLevel.Hard => Get("SudokuPage_AnalysisResultColumn_Hard"),
					DifficultyLevel.Fiendish => Get("SudokuPage_AnalysisResultColumn_Fiendish"),
					DifficultyLevel.Nightmare => Get("SudokuPage_AnalysisResultColumn_Nightmare"),
					_ => string.Empty
				},
				_ => null
			};

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws due to not implemented.</exception>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) =>
		throw new NotImplementedException();
}
