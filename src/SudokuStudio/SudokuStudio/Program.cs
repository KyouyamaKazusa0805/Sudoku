namespace SudokuStudio;

/// <summary>
/// Represents a type that is only used for surrounding with main method <c>Main</c>.
/// </summary>
internal static class Program
{
	/// <summary>
	/// Provides with the program entry point.
	/// </summary>
	/// <param name="args">The command-line arguments.</param>
	[STAThread]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	private static void Main(string[] args)
	{
		xamlCheckProcessRequirements();

		ComWrappersSupport.InitializeComWrappers();
		Application.Start(startCallback);


		static void startCallback(ApplicationInitializationCallbackParams p)
		{
			var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
			SynchronizationContext.SetSynchronizationContext(context);

			_ = new App();
		}

		[DllImport("Microsoft.ui.xaml", EntryPoint = "XamlCheckProcessRequirements")]
		static extern void xamlCheckProcessRequirements();
	}
}
