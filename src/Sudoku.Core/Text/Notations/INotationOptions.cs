namespace Sudoku.Text.Notations;

/// <summary>
/// Defines a type that provides the extra options that is used by
/// <see cref="ICellNotation{TBaseType, TOptions}"/> or <see cref="ICandidateNotation{TBaseType, TOptions}"/> instances.
/// </summary>
/// <typeparam name="TSelf">The type of the handler.</typeparam>
/// <seealso cref="ICellNotation{TBaseType, TOptions}"/>
/// <seealso cref="ICandidateNotation{TBaseType, TOptions}"/>
public interface INotationOptions<[Self] TSelf> where TSelf : struct, INotationOptions<TSelf>
{
	/// <summary>
	/// Indicates the default instance.
	/// </summary>
	static abstract TSelf Default { get; }
}
