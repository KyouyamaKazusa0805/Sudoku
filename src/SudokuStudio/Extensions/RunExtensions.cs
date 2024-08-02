namespace Microsoft.UI.Xaml.Documents;

/// <summary>
/// Provides with extension methods on <see cref="Run"/>.
/// </summary>
/// <seealso cref="Run"/>
public static class RunExtensions
{
	/// <summary>
	/// Creates a <see cref="Bold"/> instance with a singleton value of <see cref="Run"/>.
	/// </summary>
	/// <param name="this">The <see cref="Run"/> instance.</param>
	/// <returns>A <see cref="Bold"/> instance.</returns>
	public static TSpan SingletonSpan<TSpan>(this Run @this) where TSpan : Span, new()
	{
		var result = new TSpan();
		result.Inlines.Add(@this);
		return result;
	}
}
