namespace Sudoku.ComponentModel;

/// <summary>
/// Represents data that can be used in an <see cref="FullAppliedEventHandler{TSelf, TResult}"/>.
/// </summary>
/// <seealso cref="FullAppliedEventHandler{TSelf, TResult}"/>
/// <param name="appliedSteps">Indicates all applied steps.</param>
public sealed partial class FullAppliedEventArgs([Data] Step[] appliedSteps) : EventArgs;
