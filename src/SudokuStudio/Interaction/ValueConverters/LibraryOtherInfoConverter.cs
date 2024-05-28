namespace SudokuStudio.Interaction.ValueConverters;

/// <summary>
/// Represents a value converter type that can convert multiple values in <see cref="LibraryBindableSource"/>
/// into a <see cref="string"/>.
/// </summary>
/// <seealso cref="LibraryBindableSource"/>
public sealed class LibraryOtherInfoConverter : IValueConverter
{
	/// <inheritdoc/>
	public object Convert(object value, Type targetType, object parameter, string language)
		=> value switch
		{
			LibraryInfo { Tags: var tags, LastModifiedTime: var time }
				=> $"{tags switch
				{
					{ Length: not 0 } => string.Join(ResourceDictionary.Get("_Token_Comma", App.CurrentCulture), tags),
					_ => ResourceDictionary.Get("NoTags", App.CurrentCulture)
				}} | {time.ToString(App.CurrentCulture)}",
			_ => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("InvalidLibraryBindableSource"))
		};

	/// <inheritdoc/>
	[DoesNotReturn]
	public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}
