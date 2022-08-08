namespace Sudoku.UI.Views.Windows;

/// <summary>
/// A splash screen window.
/// </summary>
public sealed partial class MySplashScreen : SplashScreenBase
{
	/// <summary>
	/// Initializes a <see cref="MySplashScreen"/> instance via the specified type of the window.
	/// </summary>
	/// <param name="typeOfWindow">The type of the window.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MySplashScreen(Type typeOfWindow) : base(typeOfWindow)
	{
		InitializeComponent();
		SetProgramNameToTitle();
		SetIconFromAssetFile();
	}


	/// <inheritdoc/>
	protected override async Task OnLoading() => await Task.Delay(3000);

	/// <summary>
	/// Try to set the title.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetProgramNameToTitle() => Title = R["ProgramName"];

	/// <summary>
	/// Sets the icon from assets file.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void SetIconFromAssetFile() => this.GetAppWindow().SetIcon(@"Assets\Logo.ico");
}
