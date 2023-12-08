using System.SourceGeneration;
using SudokuStudio.Configuration;

namespace SudokuStudio.Interaction;

/// <summary>
/// Provides event data used by delegate type <see cref="ReceivedDroppedFileSuccessfullyEventHandler"/>.
/// </summary>
/// <param name="filePath">The path of the dropped file.</param>
/// <param name="gridInfo">The loaded grid info.</param>
/// <seealso cref="ReceivedDroppedFileSuccessfullyEventHandler"/>
public sealed partial class ReceivedDroppedFileSuccessfullyEventArgs([Data] string filePath, [Data] GridInfo gridInfo) : EventArgs;
