namespace Sudoku.CommandLine;

/// <summary>
/// Provides a way to create a <see cref="SymbolList{TSymbol}"/> instance.
/// </summary>
/// <seealso cref="SymbolList{TSymbol}"/>
public static class SymbolList
{
	/// <summary>
	/// Creates a list of <typeparamref name="TSymbol"/> instances.
	/// </summary>
	/// <typeparam name="TSymbol">The type of symbols.</typeparam>
	/// <param name="symbols">The symbols.</param>
	/// <returns>A <see cref="SymbolList{TSymbol}"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static SymbolList<TSymbol> Create<TSymbol>(params ReadOnlySpan<TSymbol> symbols) where TSymbol : Symbol => new(symbols);
}
