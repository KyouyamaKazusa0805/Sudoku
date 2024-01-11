namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="OpenFileFailedEventHandler"/>.
/// </summary>
/// <seealso cref="OpenFileFailedEventHandler"/>
/// <param name="reason">The failed reason.</param>
/// <remarks>
/// Initializes an <see cref="OpenFileFailedEventArgs"/> instance via the specified reason.
/// </remarks>
public sealed partial class OpenFileFailedEventArgs([RecordParameter] OpenFileFailedReason reason) : EventArgs;
