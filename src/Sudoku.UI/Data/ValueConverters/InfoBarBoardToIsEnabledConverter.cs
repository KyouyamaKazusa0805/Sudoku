using Microsoft.UI.Xaml.Data;
using Sudoku.Diagnostics.CodeAnalysis;
using Sudoku.UI.Views.Controls;

namespace Sudoku.UI.Data.ValueConverters;

/// <summary>
/// Defines a value converter that allows the one-way binding from the <see cref="InfoBarBoard.MessageCount"/>
/// to a <see cref="bool"/> vcalue indicating whether the control should be enabled.
/// </summary>
/// <seealso cref="InfoBarBoard.MessageCount"/>
public sealed class InfoBarBoardToIsEnabledConverter : IValueConverter
{
	/// <inheritdoc/>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="targetType"/> is not <see cref="bool"/>.
	/// </exception>
	[return: NotNullIfNotNull("value")]
	public object? Convert(
		object? value, Type targetType, [IsDiscard] object? parameter, [IsDiscard] string language) =>
		targetType != typeof(bool)
			? throw new ArgumentException("The desired target type must be 'bool'.", nameof(targetType))
			: value switch { int i => i != 0, _ => null };

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws due to not implemented.</exception>
	public object ConvertBack(object value, Type targetType, object parameter, string language) =>
		throw new NotImplementedException();
}
