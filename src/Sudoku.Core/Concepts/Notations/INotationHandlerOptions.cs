namespace Sudoku.Concepts.Notations;

/// <summary>
/// Defines a type that provides the extra options that is used by the <see cref="INotationHandler"/> instance.
/// </summary>
/// <typeparam name="TSelf">The type of the handler.</typeparam>
public interface INotationHandlerOptions</*[Self]*/ TSelf> : IDefaultable<TSelf>
	where TSelf : struct, IDefaultable<TSelf>
{
}
