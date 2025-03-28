namespace Sudoku.CommandLine;

/// <summary>
/// Provides with extension methods on <see cref="ParseResult"/>.
/// </summary>
/// <seealso cref="ParseResult"/>
public static class ParseResultExtensions
{
	/// <summary>
	/// Returns a list of <see cref="ReadOnlySpan{T}"/> instances representing the results parsed.
	/// </summary>
	/// <param name="this">The parse result.</param>
	/// <param name="options">The options.</param>
	/// <returns>The results.</returns>
	public static ReadOnlySpan<object?> GetOptions(this ParseResult @this, params SymbolList<Option> options)
	{
		var result = new List<object?>(options.Length);
		foreach (var option in options)
		{
			result.Add(@this.GetValueForOption(option));
		}
		return result.AsSpan();
	}
}
