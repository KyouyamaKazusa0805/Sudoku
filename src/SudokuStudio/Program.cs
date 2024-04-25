namespace SudokuStudio;

/// <summary>
/// Represents a type that encapsulates the entry point.
/// </summary>
file static class Program
{
	/// <summary>
	/// Indicates the entry point method.
	/// </summary>
	/// <param name="args">The command line arguments.</param>
	[STAThread]
	private static void Main(string[] args)
	{
		checkProcessRequirements();

		ComWrappersSupport.InitializeComWrappers();
		Application.Start(startCallback);


		[DllImport("Microsoft.ui.xaml", EntryPoint = "XamlCheckProcessRequirements")]
		static extern void checkProcessRequirements();

		static void startCallback(ApplicationInitializationCallbackParams __)
		{
			var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
			SynchronizationContext.SetSynchronizationContext(context);
			_ = new App();
		}
	}
}
