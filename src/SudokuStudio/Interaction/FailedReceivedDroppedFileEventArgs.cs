using System.SourceGeneration;

namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="FailedReceivedDroppedFileEventHandler"/>.
/// </summary>
/// <param name="reason">The failed reason.</param>
/// <remarks>
/// Initializes a <see cref="FailedReceivedDroppedFileEventArgs"/> instance via the specified reason.
/// </remarks>
/// <seealso cref="FailedReceivedDroppedFileEventHandler"/>
public sealed partial class FailedReceivedDroppedFileEventArgs([Data] FailedReceivedDroppedFileReason reason) : EventArgs;
