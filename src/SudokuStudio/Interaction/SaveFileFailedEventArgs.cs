using System.SourceGeneration;

namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="SaveFileFailedEventHandler"/>.
/// </summary>
/// <seealso cref="SaveFileFailedEventHandler"/>
/// <param name="reason">The failed reason.</param>
/// <remarks>
/// Initializes an <see cref="SaveFileFailedEventArgs"/> instance via the specified reason.
/// </remarks>
public sealed partial class SaveFileFailedEventArgs([Data] SaveFileFailedReason reason) : EventArgs;
