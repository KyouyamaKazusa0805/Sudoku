namespace Sudoku.CommandLine.ValueConverters;

/// <summary>
/// Defines a converter that can convert a <see cref="string"/> value into the target result.
/// </summary>
public sealed class GridConverter : IValueConverter
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public object? Convert(string value) => Grid.TryParse(value, out var result) ? result : null;
}
