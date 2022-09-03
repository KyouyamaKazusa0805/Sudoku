namespace Sudoku.UI;

/// <summary>
/// Indicates the type that contains the main method, which is the main entry point.
/// </summary>
public static partial class Program
{
	/// <summary>
	/// Indicates the program name.
	/// </summary>
	public const string ProgramName = "Sudoku Studio";


	/// <summary>
	/// Indicates the main entry point of the program.
	/// </summary>
	/// <param name="args">The command line arguments. The default value is an empty array.</param>
	[STAThread]
	private static void Main([SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")] string[] args)
	{
		CheckProcessRequirements();

		ComWrappersSupport.InitializeComWrappers();
		Application.Start(
			static delegate
			{
				var context = new DispatcherQueueSynchronizationContext(MsDispatcherQueue.GetForCurrentThread());
				SynchronizationContext.SetSynchronizationContext(context);
				_ = new App();
			}
		);
	}

	[LibraryImport("Microsoft.ui.xaml", EntryPoint = "XamlCheckProcessRequirements")]
	private static partial void CheckProcessRequirements();
}
