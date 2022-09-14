namespace System.Linq;

/// <summary>
/// Extracts to a new interface, indicating the type supports LINQ expressions.
/// </summary>
/// <typeparam name="TElement">The type of each element.</typeparam>
public interface ILinqProvider<out TElement> : IEnumerable<TElement>
{
}
