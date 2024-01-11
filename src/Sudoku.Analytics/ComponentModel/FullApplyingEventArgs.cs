namespace Sudoku.ComponentModel;

/// <summary>
/// Represents data that can be used in an <see cref="FullApplyingEventHandler{TSelf, TResult}"/>.
/// </summary>
/// <seealso cref="FullApplyingEventHandler{TSelf, TResult}"/>
/// <param name="applyingSteps">Indicates all steps to be applied at once.</param>
public sealed partial class FullApplyingEventArgs([RecordParameter] Step[] applyingSteps) : EventArgs;
