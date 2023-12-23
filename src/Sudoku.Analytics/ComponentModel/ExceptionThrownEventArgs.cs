namespace Sudoku.ComponentModel;

/// <summary>
/// Represents data that can be used in an <see cref="ExceptionThrownEventHandler{TSelf, TResult}"/>.
/// </summary>
/// <seealso cref="ExceptionThrownEventHandler{TSelf, TResult}"/>
/// <param name="exception">Indicates the exception having been thrown.</param>
public sealed partial class ExceptionThrownEventArgs([Data] Exception exception) : EventArgs;
