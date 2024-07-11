namespace Sudoku.Concepts.Formatting;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value
/// via the specified <see cref="MinilineBase"/> and <see cref="MinilineResult"/> instance.
/// </summary>
/// <param name="intersections">
/// <para>A list of intersections.</para>
/// <include file="../../global-doc-comments.xml" path="//g/csharp12/feature[@name='params-collections']/target[@name='parameter']"/>
/// </param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="intersections"/>.</returns>
public delegate string IntersectionNotationConverter(params ReadOnlySpan<Miniline> intersections);
