#undef HIDE_TITLE_BAR_ICON

namespace Sudoku.UI;

/// <summary>
/// The type that provides with the <see cref="Window"/> instances
/// which the current UI project starts and launches.
/// </summary>
/// <seealso cref="Window"/>
public sealed partial class MainWindow : Window
{
	/// <summary>
	/// Indiates the navigation list.
	/// </summary>
	/// <remarks>
	/// The triplet contains three values, where:
	/// <list type="table">
	/// <item>
	/// <term><c>Tag</c></term>
	/// <description>
	/// The tag of the <see cref="NavigationViewItem"/> instance, i.e. the property
	/// <see cref="FrameworkElement.Tag"/>.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>Type</c></term>
	/// <description>
	/// The type that the <see cref="Frame"/> instance bounds with a <see cref="NavigationView"/> instance
	/// can navigate and redirect to the specified page.
	/// </description>
	/// </item>
	/// <item>
	/// <term><c>Content</c></term>
	/// <description>The content of the page, i.e. the property <see cref="ContentControl.Content"/>.</description>
	/// </item>
	/// </list>
	/// </remarks>
	/// <seealso cref="NavigationView"/>
	/// <seealso cref="NavigationViewItem"/>
	/// <seealso cref="Frame"/>
	/// <seealso cref="FrameworkElement.Tag"/>
	/// <seealso cref="ContentControl.Content"/>
	private static readonly (string Tag, Type Type, string Content)[] NavigationInfoList = new[]
	{
		(
			(string)UiResources.Current.MainWindow_NavigationViewItem_Tag_SudokuPanel,
			typeof(SudokuPanelPage),
			(string)UiResources.Current.MainWindow_NavigationViewItem_Content_SudokuPanel
		),
		(
			(string)UiResources.Current.MainWindow_NavigationViewItem_Tag_About,
			typeof(AboutPage),
			(string)UiResources.Current.MainWindow_NavigationViewItem_Content_About
		)
	};


	/// <summary>
	/// Initializes a <see cref="MainWindow"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public MainWindow()
	{
		InitializeComponent();

		HideTitleBarIcon();
		InitializeControls();
	}


	/// <summary>
	/// Initializes the controls.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void InitializeControls()
	{
		Title = UiResources.Current.MainWindowTitle;
	}

	/// <summary>
	/// Hide the title bar icon.
	/// </summary>
	[Conditional("HIDE_TITLE_BAR_ICON")]
	private void HideTitleBarIcon()
	{
		const int WM_COMMAND = 0x111;

		var toggleDesktopCommand = new IntPtr(0x7402);
		sendMessage(g(), WM_COMMAND, toggleDesktopCommand, IntPtr.Zero);


		static IntPtr g()
		{
			IntPtr hShellViewWin = IntPtr.Zero, hWorkerW = IntPtr.Zero;

			var hProgman = findWindow("Progman", "Program Manager");
			var hDesktopWnd = getDesktopWindow();

			// If the main Program Manager window is found.
			if (hProgman != IntPtr.Zero)
			{
				// Get and load the main List view window containing the icons.
				hShellViewWin = findWindowEx(hProgman, IntPtr.Zero, "SHELLDLL_DefView", null);
				if (hShellViewWin == IntPtr.Zero)
				{
					// When this fails (picture rotation is turned ON, toggledesktop shell cmd used ),
					// then look for the WorkerW windows list to get the correct desktop list handle.
					// As there can be multiple WorkerW windows, iterate through all to get the correct one.
					do
					{
						hWorkerW = findWindowEx(hDesktopWnd, hWorkerW, "WorkerW", null);
						hShellViewWin = findWindowEx(hWorkerW, IntPtr.Zero, "SHELLDLL_DefView", null);
					} while (hShellViewWin == IntPtr.Zero && hWorkerW != IntPtr.Zero);
				}
			}

			return hShellViewWin;
		}

		[DllImport("user32", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
		static extern IntPtr sendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32", EntryPoint = "FindWindow", SetLastError = true)]
		static extern IntPtr findWindow(string lpClassName, string lpWindowName);

		[DllImport("user32", EntryPoint = "FindWindowEx", SetLastError = true)]
		static extern IntPtr findWindowEx(IntPtr parentHandle, IntPtr childAfter, string className, string? windowTitle);

		[DllImport("user32", EntryPoint = "GetDesktopWindow", SetLastError = false)]
		static extern IntPtr getDesktopWindow();
	}


	/// <summary>
	/// Invokes when the user switches the page to another one in the control <see cref="NavigationView_Main"/>.
	/// </summary>
	/// <param name="sender">The instance which triggered this event.</param>
	/// <param name="args">The arguments provided.</param>
	private void NavigationView_Main_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
	{
		if (
			(Sender: sender, Args: args) is not
			(
				Sender:
				{
					Header: var navigationViewHeader,
					Content: Frame
					{
						Name: nameof(Frame_NavigationView_Main),
						CurrentSourcePageType: var type
					} frame
				},
				Args:
				{
					InvokedItemContainer.Tag: string tag,
					RecommendedNavigationTransitionInfo: var info,
					IsSettingsInvoked: var settingsInvoked
				}
			)
		)
		{
			return;
		}

		var fno = new FrameNavigationOptions { TransitionInfoOverride = info };
		if (settingsInvoked)
		{
			sender.Header = UiResources.Current.MainWindow_NavigationViewItem_Content_Settings;
			frame.NavigateToType(typeof(SettingsPage), null, fno);
		}
		else
		{
			try
			{
				var (_, pageType, header) = NavigationInfoList.FirstOnThrow(triplet =>
				{
					var (a, b, c) = triplet;
					return a == tag && b != type && c is not null;
				});

				sender.Header = header;
				frame.NavigateToType(pageType, null, fno);
			}
			catch
			{
			}
		}
	}
}
