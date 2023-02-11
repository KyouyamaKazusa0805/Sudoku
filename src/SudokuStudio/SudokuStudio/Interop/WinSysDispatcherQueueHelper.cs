#if MICA_BACKDROP || ACRYLIC_BACKDROP
namespace SudokuStudio.Interop;

/// <summary>
/// Defines a helper instance that is applied to type <see cref="WinSysDispatcherQueue"/>.
/// </summary>
/// <seealso cref="WinSysDispatcherQueue"/>
internal sealed partial class WinSysDispatcherQueueHelper
{
	/// <summary>
	/// The internal field that is used for exchanging.
	/// </summary>
	private object? _dispatcherQueueController;


	/// <summary>
	/// Try to create a <see cref="WinSysDispatcherQueueController"/> instance.
	/// </summary>
	/// <seealso cref="WinSysDispatcherQueueController"/>
	public void EnsureWindowsSystemDispatcherQueueController()
	{
		if (WinSysDispatcherQueue.GetForCurrentThread() is not null)
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
		static extern int createController([In] DispatcherQueueOptions options, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object? dispatcherQueueController);
	}
}
#endif