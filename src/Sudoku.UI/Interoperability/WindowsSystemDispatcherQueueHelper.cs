namespace Sudoku.UI.Interoperability;

/// <summary>
/// Provides with a way to make Mica material works for the main window in the current project.
/// </summary>
internal sealed partial class WindowsSystemDispatcherQueueHelper
{
	/// <summary>
	/// The inner field that is used as a dispatcher queue controller.
	/// </summary>
	private object? _dispatcherQueueController = null;


	/// <summary>
	/// To ensure <see cref="DispatcherQueueController"/> works.
	/// </summary>
	public void EnsureWindowsSystemDispatcherQueueController()
	{
		if (DispatcherQueue.GetForCurrentThread() is not null)
		{
			// One already exists, so we'll just use it.
			return;
		}

		if (_dispatcherQueueController is null)
		{
			DispatcherQueueOptions options;
			options.dwSize = Marshal.SizeOf<DispatcherQueueOptions>();
			options.threadType = 2; // DQTYPE_THREAD_CURRENT
			options.apartmentType = 2; // DQTAT_COM_STA

			_ = CreateDispatcherQueueController(options, ref _dispatcherQueueController);
		}
	}


	/// <summary>
	/// Creates a <see cref="DispatcherQueueController"/>
	/// on the caller's thread. Use the created <see cref="DispatcherQueueController"/> to create
	/// and manage the lifetime of a <see cref="DispatcherQueue"/>
	/// to run queued tasks in priority order on the Dispatcher queue's thread.
	/// </summary>
	/// <param name="options">
	/// The threading affinity and type of COM apartment for the created <see cref="DispatcherQueueController"/>.
	/// See remarks for details.
	/// </param>
	/// <param name="dispatcherQueueController">
	/// <para>The created dispatcher queue controller.</para>
	/// <para><b>Please note that the <see cref="DispatcherQueueController"/> is a WinRT object.</b></para>
	/// </param>
	/// <returns>0 for success; otherwise a failure code.</returns>
	/// <remarks>
	/// <para>Introduced in Windows 10, version 1709.</para>
	/// <para>
	/// If options.threadType is <c>DQTYPE_THREAD_DEDICATED</c>, then this function creates the dedicated thread
	/// and then creates the <see cref="DispatcherQueueController"/> on that thread.
	/// The dispatcher queue event loop runs on the new dedicated thread.
	/// </para>
	/// <para>
	/// If options.threadType is <c>DQTYPE_THREAD_CURRENT</c>, then the <see cref="DispatcherQueueController"/> instance
	/// is created on the current thread. An error results if there is already a <see cref="IDispatcherQueueController"/>
	/// on the current thread. If you create a dispatcher queue on the current thread,
	/// ensure that there is a message pump running on the current thread so that the dispatcher queue can use it
	/// to dispatch tasks.
	/// </para>
	/// <para>
	/// This call does not return until the new thread and <see cref="DispatcherQueueController"/> are created.
	/// The new thread will be initialized using the specified COM apartment.
	/// </para>
	/// <para><b>
	/// The <see cref="DispatcherQueueController"/>, and its associated <see cref="DispatcherQueue"/>,
	/// are WinRT objects. See their documentation for usage details.
	/// </b></para>
	/// </remarks>
	/// <seealso cref="DispatcherQueueController"/>
	/// <seealso cref="DispatcherQueue"/>
	/// <seealso cref="IDispatcherQueueController"/>
	[DllImport("CoreMessaging")]
	private static extern nint CreateDispatcherQueueController(
		[In] DispatcherQueueOptions options,
		[In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object? dispatcherQueueController
	);
}
