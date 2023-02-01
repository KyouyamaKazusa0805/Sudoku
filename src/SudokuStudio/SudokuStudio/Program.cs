namespace SudokuStudio;

/// <summary>
/// Represents a type that is only used for surrounding with main method <c>Main</c>.
/// </summary>
internal static partial class Program
{
	/// <summary>
	/// Provides with the program entry point.
	/// </summary>
	/// <param name="args">The command-line arguments.</param>
	[STAThread]
	private static void Main(string[] args)
	{
		XamlCheckProcessRequirements();

		ComWrappersSupport.InitializeComWrappers();
		Application.Start(
			p =>
			{
				var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
				SynchronizationContext.SetSynchronizationContext(context);

				_ = new App(args);
			}
		);
	}

	[LibraryImport("Microsoft.ui.xaml")]
	private static partial void XamlCheckProcessRequirements();
}
