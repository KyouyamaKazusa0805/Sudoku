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
/// <see cref="AnalysisResultRow.DifficultyLevel"/> to <see cref="Control.Background"/> property value.
/// </summary>
/// <seealso cref="AnalysisResultRow.DifficultyLevel"/>
/// <seealso cref="Control.Background"/>
public sealed class DifficultyLevelToBackgroundConverter : IValueConverter
{
	/// <summary>
	/// Defines the background brushes.
	/// </summary>
	private static readonly SolidColorBrush[] Backgrounds =
	{
		new(Color.FromArgb(255, 204, 204, 255)),
		new(Color.FromArgb(255, 100, 255, 100)),
		new(Color.FromArgb(255, 255, 255, 100)),
		new(Color.FromArgb(255, 255, 150,  80)),
		new(Color.FromArgb(255, 255, 100, 100))
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
				DifficultyLevel d => Backgrounds[Log2((byte)d)],
				_ => null
			};

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws due to not implemented.</exception>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) =>
		throw new NotImplementedException();
}