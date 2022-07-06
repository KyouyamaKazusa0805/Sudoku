namespace Sudoku.UI;

/// <summary>
/// Indicates the type that contains the main method, which is the main entry point.
/// </summary>
public static class Program
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
	private static void Main(string[] args)
	{
		checkProcessRequirements();

		ComWrappersSupport.InitializeComWrappers();
		Application.Start(
			static delegate
			{
				var context = new DispatcherQueueSynchronizationContext(MsDispatcherQueue.GetForCurrentThread());
				SynchronizationContext.SetSynchronizationContext(context);
				_ = new App();
			}
		);


		[DllImport("Microsoft.ui.xaml", EntryPoint = "XamlCheckProcessRequirements")]
		static extern void checkProcessRequirements();
	}
}
