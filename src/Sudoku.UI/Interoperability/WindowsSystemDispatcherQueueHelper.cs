namespace Sudoku.UI.Interoperability;

internal sealed class WindowsSystemDispatcherQueueHelper
{
	private object? _dispatcherQueueController;


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
			options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
			options.threadType = 2;    // DQTYPE_THREAD_CURRENT
			options.apartmentType = 2; // DQTAT_COM_STA

			_ = createController(options, ref _dispatcherQueueController);
		}


		[DllImport("CoreMessaging", EntryPoint = "CreateDispatcherQueueController")]
		static extern int createController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object? dispatcherQueueController);
	}
}
