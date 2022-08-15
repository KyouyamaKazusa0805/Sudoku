namespace Sudoku.UI;

/// <summary>
/// Provides application-specific behavior to supplement the default <see cref="Application"/> class.
/// </summary>
/// <seealso cref="Application"/>
public partial class App : Application
{
	/// <summary>
	/// <para>Initializes the singleton application object.</para>
	/// <para>
	/// This is the first line of authored code executed,
	/// and as such is the logical equivalent of <c>main()</c> or <c>WinMain()</c>.
	/// </para>
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public App() => InitializeComponent();


	/// <summary>
	/// Indicates the initial information.
	/// </summary>
	internal WindowInitialInfo InitialInfo { get; } = new();

	/// <summary>
	/// Indicates the runtime information.
	/// </summary>
	internal WindowRuntimeInfo RuntimeInfo { get; } = new();


	/// <summary>
	/// Indicates the user preference instance.
	/// </summary>
	internal Preference UserPreference
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => RuntimeInfo.UserPreference;
	}


	/// <summary>
	/// <para>Invoked when the application is launched normally by the end user.</para>
	/// <para>
	/// Other entry points will be used such as when the application is launched to open a specific file.
	/// </para>
	/// </summary>
	/// <param name="args">Details about the launch request and process.</param>
	protected override void OnLaunched(MsLaunchActivatedEventArgs args)
	{
		// Binds the resource fetcher on type 'MergedResources'.
		R.AddExternalResourceFetecher(new[] { GetType().Assembly, typeof(WindowExtensions).Assembly }, valueSelector);

		// Handle and assign the initial value, to control the initial page information.
		(
			AppInstance.GetCurrent().GetActivatedEventArgs() switch
			{
				{
					Kind: ExtendedActivationKind.File,
					Data: IFileActivatedEventArgs { Files: [StorageFile { FileType: var fileType } file, ..] }
				} => fileType switch
				{
					CommonFileExtensions.Sudoku => async (i, _) => i.FirstGrid = Grid.Parse(await readAsync(file)),
					CommonFileExtensions.PreferenceBackup => async (i, r) => await backPreferenceFiles(i, r, file),
					CommonFileExtensions.DrawingData => async (i, _) => i.DrawingDataRawValue = await readAsync(file),
					_ => null
				},
				_ => default(Action<WindowInitialInfo, WindowRuntimeInfo>?)
			}
		)?.Invoke(InitialInfo, RuntimeInfo);

		var splashScreen = new MySplashScreen(typeof(MainWindow));
		splashScreen.Completed += (_, e) => RuntimeInfo.MainWindow = (MainWindow)e!;


		static string? valueSelector(string key)
			=> Current.Resources.TryGetValue(key, out object? t) && t is string r ? r : null;

		static async Task backPreferenceFiles(WindowInitialInfo i, WindowRuntimeInfo r, IStorageFile file)
		{
			i.FromPreferenceFile = true;

			string content = await readAsync(file);
			var up = Deserialize<Preference>(content, CommonSerializerOptions.CamelCasing);
			r.UserPreference.CoverPreferenceBy(up);
		}

		static async Task<string> readAsync(IStorageFile file) => await FileIO.ReadTextAsync(file);
	}
}
