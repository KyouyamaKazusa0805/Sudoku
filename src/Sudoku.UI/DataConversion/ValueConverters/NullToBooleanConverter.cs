namespace Sudoku.UI.DataConversion.ValueConverters;

/// <summary>
/// Converts a null value to a <see cref="bool"/> value.
/// </summary>
public sealed class NullToBooleanConverter : IValueConverter
{
	/// <summary>
	/// Indicates whether the value conversion is reverted.
	/// </summary>
	public bool IsInverted { get; set; }

	/// <summary>
	/// Indicates whether the converter will handle for whitespaces.
	/// </summary>
	public bool EnforceNonWhiteSpaceString { get; set; }


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public object Convert(object? value, Type targetType, object parameter, string language)
	{
		if (value?.GetType() == typeof(string))
		{
			if (IsInverted)
			{
				if (EnforceNonWhiteSpaceString)
				{
					return !string.IsNullOrWhiteSpace((string)value);
				}

				return !string.IsNullOrEmpty((string)value);
			}

			if (EnforceNonWhiteSpaceString)
			{
				return !string.IsNullOrWhiteSpace((string)value);
			}

			return string.IsNullOrEmpty((string)value);
		}

		if (IsInverted)
		{
			return value is not null;
		}

		return value is null;
	}

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object? value, Type? targetType, object? parameter, string? language)
		=> throw new NotImplementedException();
}
