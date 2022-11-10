namespace Sudoku.Maui.Markup;

/// <summary>
/// Defines a text resource fetcher.
/// </summary>
[ContentProperty(nameof(Key))]
public sealed class TextExtension : IMarkupExtension<string>, IMarkupExtension
{
	/// <summary>
	/// The resource key.
	/// </summary>
	public string Key { get; set; } = string.Empty;


	/// <inheritdoc/>
	public string ProvideValue(IServiceProvider serviceProvider) => FetchTextResource(Key);

	/// <inheritdoc/>
	object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);


	/// <summary>
	/// The internal application resource text fetcher.
	/// </summary>
	/// <param name="key">The key.</param>
	/// <returns>The target value fetched.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static string FetchTextResource(string key)
		=> Application.Current switch
		{
			{ Resources: var r } when r.TryGetValue(key, out var raw) && raw is string result => result,
			_ => string.Empty
		};
}
