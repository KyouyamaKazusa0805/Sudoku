namespace Sudoku.Concepts.Formatting;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value via the specified <see cref="Conclusion"/> instance.
/// </summary>
/// <param name="conclusions">
/// <para>A list of conjugate pairs.</para>
/// <include file="../../global-doc-comments.xml" path="//g/csharp12/feature[@name='params-collections']/target[@name='parameter']"/>
/// </param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="conclusions"/>.</returns>
public delegate string ConclusionNotationConverter(params ReadOnlySpan<Conclusion> conclusions);
