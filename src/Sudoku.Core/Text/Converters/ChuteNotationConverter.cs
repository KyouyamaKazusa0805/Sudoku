namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value via the specified <see cref="Chute"/> instance.
/// </summary>
/// <param name="chutes">
/// <para>A list of chutes.</para>
/// <include file="../../global-doc-comments.xml" path="//g/csharp12/feature[@name='params-collections']/target[@name='parameter']"/>
/// </param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="chutes"/>.</returns>
public delegate string ChuteNotationConverter(params ReadOnlySpan<Chute> chutes);
