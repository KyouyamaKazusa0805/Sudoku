#if MICA_BACKDROP
namespace SudokuStudio.Views.Windows;

partial class MainWindow
{
	/// <summary>
	/// Defines a <see cref="WindowsSystemDispatcherQueueHelper"/> instance that is used for interaction
	/// with <see cref="winsys::DispatcherQueue"/>.
	/// </summary>
	/// <seealso cref="winsys::DispatcherQueue"/>
	private WindowsSystemDispatcherQueueHelper? _wsdqHelper;

	/// <summary>
	/// Indicates the Mica controler instance. This instance is used as core implementation of Mica material of applications.
	/// </summary>
	private MicaController? _micaController;

	/// <summary>
	/// Indicates the material configuration instance. This field controls displaying with a customized material such as Mica and acrylic.
	/// </summary>
	private SystemBackdropConfiguration? _configurationSource;


	/// <summary>
	/// Sets <see cref="SystemBackdropConfiguration.Theme"/> to the target value.
	/// </summary>
	/// <exception cref="NotSupportedException">Throws when the actual theme is not defined.</exception>
	private void SetConfigurationSourceTheme()
	{
		Debug.Assert(_configurationSource is not null);

		_configurationSource.Theme = f(((FrameworkElement)Content).ActualTheme);


		static SystemBackdropTheme f(ElementTheme elementTheme, [CallerArgumentExpression(nameof(elementTheme))] string? expression = null)
			=> elementTheme switch
			{
				ElementTheme.Dark => SystemBackdropTheme.Dark,
				ElementTheme.Light => SystemBackdropTheme.Light,
				ElementTheme.Default => SystemBackdropTheme.Default,
				_ => throw new NotSupportedException($"The value '{expression}' is not supported.")
			};
	}

	/// <summary>
	/// Try to set Mica backdrop.
	/// </summary>
	/// <returns>A <see cref="bool"/> result indicating whether the operation is succeeded.</returns>
	[MemberNotNullWhen(true, nameof(_wsdqHelper))]
	private bool TrySetMicaBackdrop()
	{
		if (DesktopAcrylicController.IsSupported())
		{
			(_wsdqHelper = new()).EnsureWindowsSystemDispatcherQueueController();

			// Hooking up the policy object.
			_configurationSource = new();

			Activated += Window_Activated;
			Closed += Window_Closed;
			((FrameworkElement)Content).ActualThemeChanged += Window_ThemeChanged;

			// Initial configuration state.
			_configurationSource.IsInputActive = true;
			SetConfigurationSourceTheme();

			_micaController = new();

			// Enable the system backdrop.
			_micaController.AddSystemBackdropTarget(this.As<ICompositionSupportsSystemBackdrop>());
			_micaController.SetSystemBackdropConfiguration(_configurationSource);
			return true; // Succeeded.
		}

		return false; // Mica is not supported on this system.
	}


	private void Window_Activated(object sender, mxaml::WindowActivatedEventArgs args)
	{
		Debug.Assert(_configurationSource is not null);

		_configurationSource.IsInputActive = args.WindowActivationState != WindowActivationState.Deactivated;
	}

	private void Window_Closed(object sender, WindowEventArgs args)
	{
		// Make sure any Mica/Acrylic controller is disposed so it doesn't try to use this closed window.
		if (_micaController is not null)
		{
			_micaController.Dispose();
			_micaController = null;
		}

		Activated -= Window_Activated;
		_configurationSource = null;
	}

	private void Window_ThemeChanged(FrameworkElement sender, object args)
	{
		if (_configurationSource is not null)
		{
			SetConfigurationSourceTheme();
		}
	}
}
#endif