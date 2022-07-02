namespace Sudoku.UI.Interop;

/// <summary>
/// Specifies the threading and apartment type for a new <see cref="WinsysDispatcherQueueController"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct DispatcherQueueOptions
{
	/// <summary>
	/// Size of this <see cref="DispatcherQueueOptions"/> structure.
	/// </summary>
	internal int dwSize;

	/// <summary>
	/// Thread affinity for the created <see cref="WinsysDispatcherQueueController"/>.
	/// </summary>
	/// <seealso cref="WinsysDispatcherQueueController"/>
	internal int threadType;

	/// <summary>
	/// Specifies whether to initialize COM apartment on the new thread
	/// as an application single-threaded apartment (ASTA) or single-threaded apartment (STA).
	/// This field is only relevant if threadType is <c>DQTYPE_THREAD_DEDICATED</c>.
	/// Use <c>DQTAT_COM_NONE</c> when <see cref="threadType"/> is <c>DQTYPE_THREAD_CURRENT</c>.
	/// </summary>
	/// <seealso cref="threadType"/>
	/// <remarks>
	/// Introduced in Windows 10, version 1709.
	/// </remarks>
	internal int apartmentType;
}
