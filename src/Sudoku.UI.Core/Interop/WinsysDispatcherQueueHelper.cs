namespace Sudoku.UI.Interop;

/// <summary>
/// Provides with a type that holds the light-weighted helper methods to controls the dispatcher queue controller.
/// </summary>
/// <seealso cref="WinsysDispatcherQueue"/>
internal sealed class WinsysDispatcherQueueHelper
{
	/// <summary>
	/// The dispatcher queue controller instance.
	/// </summary>
	private object? _dispatcherQueueController;


	/// <summary>
	/// To ensure <see cref="WinsysDispatcherQueue"/> instance contains the controller.
	/// </summary>
	public void EnsureWindowsSystemDispatcherQueueController()
	{
		if (WinsysDispatcherQueue.GetForCurrentThread() is not null)
		{
			// One already exists, so we'll just use it.
			return;
		}

		if (_dispatcherQueueController is null)
		{
			DispatcherQueueOptions options;
			options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
			options.threadType = 2; // DQTYPE_THREAD_CURRENT
			options.apartmentType = 2; // DQTAT_COM_STA

			_ = createController(options, ref _dispatcherQueueController);
		}


		[DllImport("CoreMessaging", EntryPoint = "CreateDispatcherQueueController")]
		static extern int createController(
			[In] DispatcherQueueOptions options,
			[In, Out, MarshalAs(UnmanagedType.IUnknown), AllowNull] ref object dispatcherQueueController
		);
	}
}
