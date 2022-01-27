namespace Sudoku.UI.Converters;

/// <summary>
/// Encapsulates the converter that converts the specified <see cref="string"/>
/// value as the path and the <see cref="Brush"/> as the result.
/// </summary>
/// <seealso cref="Brush"/>
public sealed class Path2BrushConverter : IValueConverter
{
	/// <summary>
	/// Indicates the prefix of the <c>ms-appx</c> protocol.
	/// </summary>
	private const string MsAppxProtocol = "ms-appx:///";


	/// <inheritdoc/>
	/// <exception cref="ArgumentException">
	/// Throws when the argument <paramref name="value"/> is not a <see cref="string"/> value.
	/// The argument <paramref name="value"/> can be <see langword="null"/>.
	/// </exception>
	public object Convert(object? value, [IsDiscard] Type targetType, [IsDiscard] object? parameter, [IsDiscard] string? language)
	{
		return value switch
		{
			null => new SolidColorBrush(Colors.DimGray),
			string uri => new ImageBrush
			{
				ImageSource = new BitmapImage(new(h(uri))),
				Stretch = Stretch.Fill
			},
			_ => throw new ArgumentException(
				message: $"The argument must be of type '{nameof(String)}'.",
				paramName: nameof(value),
				innerException: new InvalidCastException("Cannot convert the value into 'string'.")
			)
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		static string h(string uri) => uri.StartsWith(MsAppxProtocol) ? uri : $"{MsAppxProtocol}{uri}";
	}

	/// <inheritdoc/>
	/// <exception cref="NotSupportedException">Throws always.</exception>
	public object ConvertBack(object value, Type targetType, object parameter, string language) =>
		throw new NotSupportedException("This converter does not support the convert-back operation.");
}
