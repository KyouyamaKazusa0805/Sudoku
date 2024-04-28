namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a delegate type that creates a <see cref="string"/> value
/// via the specified <see cref="IntersectionBase"/> and <see cref="IntersectionResult"/> instance.
/// </summary>
/// <param name="intersections">
/// <para>A list of intersections.</para>
/// <include file="../../global-doc-comments.xml" path="//g/csharp12/feature[@name='params-collections']/target[@name='parameter']"/>
/// </param>
/// <returns>An equivalent <see cref="string"/> value to the specified argument <paramref name="intersections"/>.</returns>
public delegate string IntersectionNotationConverter(params ReadOnlySpan<Intersection> intersections);
