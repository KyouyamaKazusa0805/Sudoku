#if MICA_BACKDROP || ACRYLIC_BACKDROP
namespace SudokuStudio.Interop;

/// <summary>
/// Specifies the threading and apartment type for a new
/// <see href="https://learn.microsoft.com/en-us/uwp/api/windows.system.dispatcherqueuecontroller"><c>DispatcherQueueController</c></see>.
/// </summary>
/// <remarks>
/// <para>Introduced in Windows 10, version 1709.</para>
/// <para><b>Requirements</b></para>
/// <para>
/// <list type="table">
/// <item>
/// <term>Header</term>
/// <description><c>dispatcherqueue.h</c></description>
/// </item>
/// </list>
/// </para>
/// <para>
/// Source: <see href="https://learn.microsoft.com/en-us/windows/win32/api/dispatcherqueue/ns-dispatcherqueue-dispatcherqueueoptions">DispatcherQueueOptions structure (dispatcherqueue.h)</see>.
/// </para>
/// </remarks>
/// <seealso href="https://learn.microsoft.com/en-us/windows/win32/api/dispatcherqueue/nf-dispatcherqueue-createdispatcherqueuecontroller">
/// <c>CreateDispatcherQueueController</c>
/// </seealso>
[StructLayout(LayoutKind.Sequential)]
internal struct DispatcherQueueOptions
{
	/// <summary>
	/// Size of this <c><b>DispatcherQueueOptions</b></c> structure.
	/// </summary>
	internal int dwSize;

	/// <summary>
	/// Thread affinity for the created <c><b>DispatcherQueueController</b></c>.
	/// </summary>
	internal int threadType;

	/// <summary>
	/// Specifies whether to initialize COM apartment on the new thread as an application single-threaded apartment (ASTA)
	/// or single-threaded apartment (STA). This field is only relevant if <see cref="threadType"/> is <c>DQTYPE_THREAD_DEDICATED</c>.
	/// Use <c>DQTAT_COM_NONE</c> when <see cref="threadType"/> is <c>DQTYPE_THREAD_CURRENT</c>.
	/// </summary>
	internal int apartmentType;
}
#endif