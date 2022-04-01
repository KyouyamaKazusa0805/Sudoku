using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Sudoku.Solving.Manual;
using Windows.UI;
using static System.Numerics.BitOperations;

namespace Sudoku.UI.Data.ValueConverters;

/// <summary>
/// Defines a value converter that allows the one-way binding from the
/// <see cref="AnalysisResultRow.DifficultyLevel"/> to <see cref="Control.Foreground"/> property value.
/// </summary>
/// <seealso cref="AnalysisResultRow.DifficultyLevel"/>
/// <seealso cref="Control.Foreground"/>
public sealed class DifficultyLevelToForegroundConverter : IValueConverter
{
	/// <summary>
	/// Defines the foreground brushes.
	/// </summary>
	private static readonly SolidColorBrush[] Foregrounds =
	{
		new(Color.FromArgb(255,   0,  51, 204)),
		new(Color.FromArgb(255,   0, 102,   0)),
		new(Color.FromArgb(255, 102,  51,   0)),
		new(Color.FromArgb(255, 102,  51,   0)),
		new(Color.FromArgb(255, 102,   0,   0))
	};


	/// <inheritdoc/>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="targetType"/> is not <see cref="Brush"/>.
	/// </exception>
	[return: NotNullIfNotNull("value")]
	public object? Convert(object? value, Type targetType, object? parameter, string language) =>
		targetType != typeof(Brush)
			? throw new ArgumentException($"The desired target type must be '{nameof(Brush)}'.", nameof(targetType))
			: value switch
			{
				DifficultyLevel and (0 or > DifficultyLevel.Nightmare) => new(Colors.Transparent),
				DifficultyLevel d => Foregrounds[Log2((byte)d)],
				_ => null
			};

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws due to not implemented.</exception>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) =>
		throw new NotImplementedException();
}
