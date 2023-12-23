namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="ReceivedDroppedFileFailedEventHandler"/>.
/// </summary>
/// <param name="reason">The failed reason.</param>
/// <remarks>
/// Initializes a <see cref="ReceivedDroppedFileFailedEventArgs"/> instance via the specified reason.
/// </remarks>
/// <seealso cref="ReceivedDroppedFileFailedEventHandler"/>
public sealed partial class ReceivedDroppedFileFailedEventArgs([Data] ReceivedDroppedFileFailedReason reason) : EventArgs;
