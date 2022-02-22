using Windows.Storage.Pickers;

namespace Microsoft.UI.Xaml.Controls;

/// <summary>
/// Provides the extension methods on <see cref="FileOpenPicker"/>.
/// </summary>
/// <seealso cref="FileOpenPicker"/>
internal static class FileOpenPickerExtensions
{
	/// <summary>
	/// Adds the file type filter into the <see cref="FileOpenPicker"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="FileOpenPicker"/> instance.</param>
	/// <param name="extension">The extension to be added.</param>
	/// <returns>The reference that is same as argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileOpenPicker AddFileTypeFilter(this FileOpenPicker @this, string extension)
	{
		@this.FileTypeFilter.Add(extension);
		return @this;
	}

	/// <summary>
	/// Adds the file type filters into the <see cref="FileOpenPicker"/> instance.
	/// </summary>
	/// <param name="this">The <see cref="FileOpenPicker"/> instance.</param>
	/// <param name="extensions">The extensions to be added.</param>
	/// <returns>The reference that is same as argument <paramref name="this"/>.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static FileOpenPicker AddFileTypeFilters(this FileOpenPicker @this, params string[] extensions)
	{
		@this.FileTypeFilter.AddRange(extensions);
		return @this;
	}
}
