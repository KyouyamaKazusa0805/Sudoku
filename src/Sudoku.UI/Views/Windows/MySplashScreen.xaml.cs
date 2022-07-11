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
	public MySplashScreen(Type typeOfWindow) : base(typeOfWindow) => InitializeComponent();


	/// <inheritdoc/>
	protected override async Task OnLoading() => await Task.Delay(3000);
}
