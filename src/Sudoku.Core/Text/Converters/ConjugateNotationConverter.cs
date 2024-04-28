namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value via the specified <see cref="Conjugate"/> instance.
/// </summary>
/// <param name="conjugatePairs">
/// <para>A list of conjugate pairs.</para>
/// <include file="../../global-doc-comments.xml" path="//g/csharp12/feature[@name='params-collections']/target[@name='parameter']"/>
/// </param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="conjugatePairs"/>.</returns>
public delegate string ConjugateNotationConverter(params ReadOnlySpan<Conjugate> conjugatePairs);
