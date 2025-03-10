namespace Sudoku.Runtime.InterceptorServices;

/// <summary>
/// Represents an attribute type that can be applied to a method,
/// to tell interceptor source generator to produce a new method that copies full implementation from this method,
/// with <see cref="Grid"/> properties replaced with corresponding properties in <see cref="MemoryCachedData"/>.
/// </summary>
/// <remarks>
/// Usage:
/// <code><![CDATA[
/// [Cached]
/// public static ReadOnlySpan<AlmostLockedSetPattern> Collect(in Grid grid)
/// {
///	    // VARIABLE_DECLARATION_BEGIN
///     _ = grid is { EmptyCells: var __EmptyCells, BivalueCells: var __BivalueCells, CandidatesMap: var __CandidatesMap };
///     // VARIABLE_DECLARATION_END
///
///     // Get all bi-value-cell ALSes.
///     var result = new List<AlmostLockedSetPattern>();
///     foreach (var cell in __BivalueCells)
///     {
///         var eliminationMap = new CellMap[9];
///         foreach (var digit in grid.GetCandidates(cell))
///             eliminationMap[digit] = PeersMap[cell] & __CandidatesMap[digit];
///         result.Add(new(grid.GetCandidates(cell), cell.AsCellMap(), PeersMap[cell] & __EmptyCells, eliminationMap));
///     }
///
///     // ...
/// }
/// ]]></code>
/// Such code will be intercepted, by replacing variables <c>__EmptyCells</c>, <c>__BivalueCells</c> and <c>__CandidatesMap</c>
/// with properties in <see cref="MemoryCachedData"/>:
/// <code><![CDATA[
/// // Get all bi-value-cell ALSes.
/// var result = new List<AlmostLockedSetPattern>();
/// foreach (var cell in MemoryCachedData.BivalueCells)
/// {
///     var eliminationMap = new CellMap[9];
///     foreach (var digit in grid.GetCandidates(cell))
///         eliminationMap[digit] = PeersMap[cell] & MemoryCachedData.CandidatesMap[digit];
///     result.Add(new(grid.GetCandidates(cell), cell.AsCellMap(), PeersMap[cell] & MemoryCachedData.EmptyCells, eliminationMap));
/// }
/// ]]></code>
/// </remarks>
/// <seealso cref="Grid"/>
/// <seealso cref="MemoryCachedData"/>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class CachedAttribute : Attribute;
