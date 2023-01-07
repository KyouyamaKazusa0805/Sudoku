namespace SudokuStudio;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
/// <seealso cref="Application"/>
public partial class App : Application
{
	/// <summary>
	/// <para>Initializes the singleton application object.</para>
	/// <para>
	/// This is the first line of authored code executed, and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	public App() => InitializeComponent();


	/// <summary>
	/// Indicates the running context.
	/// </summary>
	public RunningContext RunningContext { get; } = new();


	/// <inheritdoc/>
	protected override void OnLaunched(LaunchActivatedEventArgs args)
	{
		var assembly = GetType().Assembly;
		R.RegisterAssembly(assembly);

		(RunningContext.MainWindow = new MainWindow()).Activate();
	}
}
