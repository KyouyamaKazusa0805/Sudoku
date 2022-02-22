namespace Windows.Storage.Pickers;

/// <summary>
/// Provides the extension methods on <see cref="FileSavePicker"/>.
/// </summary>
/// <seealso cref="FileSavePicker"/>
internal static class FileSavePickerWithExtensions
{
	/// <summary>
	/// Adds the suggested file name into the specified <see cref="FileSavePicker"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="FileSavePicker"/> instance.</param>
	/// <param name="suggestedFileName">The suggested file name.</param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileSavePicker WithSuggestedFileName(this FileSavePicker @this, string suggestedFileName)
	{
		@this.SuggestedFileName = suggestedFileName;
		return @this;
	}

	/// <summary>
	/// Adds a new file type filter into the specified <see cref="FileSavePicker"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="FileSavePicker"/> instance.</param>
	/// <param name="name">The name of the file extension set.</param>
	/// <param name="extensions">The file extensions that belongs to <paramref name="name"/>.</param>
	/// <returns>The reference that is same as the argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileSavePicker AddFileTypeFilter(
		this FileSavePicker @this, string name, IList<string> extensions)
	{
		@this.FileTypeChoices.Add(name, extensions);
		return @this;
	}
}
