namespace Sudoku.UI.Interoperability;

partial class WindowsSystemDispatcherQueueHelper
{
	/// <summary>
	/// Specifies the threading and apartment type for a new <see cref="DispatcherQueueController"/>.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	private struct DispatcherQueueOptions
	{
		/// <summary>
		/// Size of this <see cref="DispatcherQueueOptions"/> structure.
		/// </summary>
		internal nint dwSize;

		/// <summary>
		/// Thread affinity for the created <see cref="DispatcherQueueController"/>.
		/// </summary>
		/// <seealso cref="DispatcherQueueController"/>
		internal nint threadType;

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
		internal nint apartmentType;
	}
}
