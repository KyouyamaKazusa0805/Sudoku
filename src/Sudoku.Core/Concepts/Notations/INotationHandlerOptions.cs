namespace Sudoku.Concepts.Notations;

/// <summary>
/// Defines a type that provides the extra options that is used by the <see cref="INotationHandler"/> instance.
/// </summary>
/// <typeparam name="T">The type of the handler.</typeparam>
public interface INotationHandlerOptions</*[Self]*/ T> : IDefaultable<T> where T : struct, IDefaultable<T>
{
}
