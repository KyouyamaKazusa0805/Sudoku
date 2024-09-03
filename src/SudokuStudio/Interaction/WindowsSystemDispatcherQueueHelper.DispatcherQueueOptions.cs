namespace SudokuStudio.Interaction;

internal sealed partial class WindowsSystemDispatcherQueueHelper
{
	/// <summary>
	/// Represents options around threading affinity and type of COM apartment for a new <see cref="DispatcherQueueController"/>.
	/// </summary>
	/// <remarks>
	/// Introduced in Windows 10, version 1709.
	/// </remarks>
	/// <seealso cref="CreateDispatcherQueueController"/>
	[StructLayout(LayoutKind.Sequential)]
	private struct DispatcherQueueOptions
	{
		/// <summary>
		/// Size of this <see cref="DispatcherQueueOptions"/> structure.
		/// </summary>
		internal int dwSize;

		/// <summary>
		/// Thread affinity for a new <see cref="DispatcherQueueController"/>.
		/// </summary>
		internal int threadType;

		/// <summary>
		/// Specifies whether to initialize the COM apartment on the new thread as an application single-threaded apartment (ASTA)
		/// or a single-threaded apartment (STA).
		/// This field is relevant only if threadType is <c>DQTYPE_THREAD_DEDICATED</c>.
		/// Use <c>DQTAT_COM_NONE</c> when <see cref="threadType"/> is <c>DQTYPE_THREAD_CURRENT</c>.
		/// </summary>
		internal int apartmentType;
	}
}
