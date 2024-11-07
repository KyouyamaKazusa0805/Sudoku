namespace Sudoku.Analytics.Intercepting;

/// <summary>
/// Represents an attribute type that can be applied to a method,
/// making source generator produces a new method that copies the code from this,
/// with <see cref="Grid"/> properties replaced with corresponding properties in <see cref="MemoryCachedData"/>.
/// </summary>
/// <remarks>
/// Usage:
/// <code><![CDATA[
/// [Cached]
/// public static ReadOnlySpan<AlmostLockedSetPattern> Collect(ref readonly Grid grid)
/// {
///	    // --INTERCEPTOR_VARIABLE_DECLARATION_BEGIN--
///     _ = grid is { EmptyCells: var __EmptyCells, BivalueCells: var __BivalueCells, CandidatesMap: var __CandidatesMap };
///     // --INTERCEPTOR_VARIABLE_DECLARATION_END--
///
///     // Get all bi-value-cell ALSes.
///     var result = new List<AlmostLockedSetPattern>();
///     foreach (var cell in __BivalueCells)
///     {
///         var eliminationMap = new CellMap[9];
///         foreach (var digit in grid.GetCandidates(cell))
///             eliminationMap[digit] = PeersMap[cell] & __CandidatesMap[digit];
///         result.Add(new(grid.GetCandidates(cell), in cell.AsCellMap(), PeersMap[cell] & __EmptyCells, eliminationMap));
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
///     result.Add(new(grid.GetCandidates(cell), in cell.AsCellMap(), PeersMap[cell] & MemoryCachedData.EmptyCells, eliminationMap));
/// }
/// ]]></code>
/// </remarks>
/// <seealso cref="Grid"/>
/// <seealso cref="MemoryCachedData"/>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class CachedAttribute : Attribute;
