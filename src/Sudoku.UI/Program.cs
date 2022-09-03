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
	/// <param name="args">
	/// <para>The command line arguments. The default value is an empty array.</para>
	/// <para><i>
	/// This method does not use command-line arguments, but it will be reserved in order to be used in the future.
	/// </i></para>
	/// </param>
	[STAThread]
	[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
	private static void Main(string[] args)
	{
		CheckProcessRequirements();

		ComWrappersSupport.InitializeComWrappers();
		Application.Start(ApplicationStartingCallback);
	}

	/// <summary>
	/// The callback method that is called by main method. <i>Please do not call this method in other places.</i>
	/// </summary>
	/// <param name="p">The callback method arguments.</param>
	private static void ApplicationStartingCallback(ApplicationInitializationCallbackParams p)
	{
		var context = new DispatcherQueueSynchronizationContext(MsDispatcherQueue.GetForCurrentThread());
		SynchronizationContext.SetSynchronizationContext(context);
		_ = new App();
	}

	[LibraryImport("Microsoft.ui.xaml", EntryPoint = "XamlCheckProcessRequirements")]
	private static partial void CheckProcessRequirements();
}
