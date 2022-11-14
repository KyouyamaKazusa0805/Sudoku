namespace Sudoku.Text.Notations;

/// <summary>
/// Defines a type that provides the extra options that is used by
/// <see cref="ICellNotation{TSelf, TOptions}"/> or <see cref="ICandidateNotation{TSelf, TOptions}"/> instances.
/// </summary>
/// <typeparam name="TSelf">The type of the handler.</typeparam>
/// <seealso cref="ICellNotation{TSelf, TOptions}"/>
/// <seealso cref="ICandidateNotation{TSelf, TOptions}"/>
public interface INotationOptions<[Self] TSelf> where TSelf : struct, INotationOptions<TSelf>
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	static abstract TSelf Default { get; }
}
