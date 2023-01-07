namespace SudokuStudio.Views.Windows;

partial class MainWindow
{
	private WindowsSystemDispatcherQueueHelper? _wsdqHelper;

	private MicaController? _micaController;

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

		return false; // Acrylic is not supported on this system.
	}


	private void Window_Activated(object sender, WindowActivatedEventArgs args)
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
