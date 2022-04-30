namespace Sudoku.Bot.Oicq.Extensibility;

/// <summary>
/// Provides with the Amiable service as the base service. The type will be used automatically.
/// </summary>
public static class AmiableService
{
	/// <summary>
	/// Indicates the API key.
	/// </summary>
	public static string ApiKey = null!;

	/// <summary>
	/// Indicates the app service instance.
	/// </summary>
	public static AppService App = null!;

	/// <summary>
	/// Indicates the events handler.
	/// </summary>
	public static List<IPluginEventHandler> EventHandlers = new();


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static AmiableService()
	{
		static void f(UnhandledExceptionEventArgs e) => App.Log($"[{nameof(AppDomain)}Error]", e.ExceptionObject.ToString());
		AppDomain.CurrentDomain.UnhandledException += static (_, e) => f(e);

		// Initializes the fields and properties.
		App = new AppService();
		SetAppInfo();
		ServiceBuilder(App);
		RegisterPluginModule();

		// Invokes the event.
		BackingEventHandler.InvokeEvent(AmiableEventType.AmiableLoaded, new());
		App.Log($"[{nameof(AppDomain)}]", AppDomain.CurrentDomain.FriendlyName);
	}


	/// <summary>
	/// Sets the app info.
	/// </summary>
	public static void SetAppInfo()
		=> App.AppInfo = new("Sudoku.Bot.Oicq.Exporting", "0.1.0", "SunnieShine", "Sudoku bot", "sunnie.app.sudoku");

	/// <summary>
	/// Builds the services and make compatible platform configurations.
	/// </summary>
	/// <param name="service">The current service.</param>
	public static void ServiceBuilder(AppService service) => service.AddConfig();

	/// <summary>
	/// Adds the specified <see cref="IPluginEventHandler"/> instance into the event handler collection.
	/// </summary>
	/// <typeparam name="TPluginEventHandler">The type of the plugin event handler.</typeparam>
	public static void AddPluginEventHandler<TPluginEventHandler>() where TPluginEventHandler : IPluginEventHandler, new()
		=> EventHandlers.Add(Activator.CreateInstance<TPluginEventHandler>());

	/// <summary>
	/// Registers the plugin module.
	/// </summary>
	private static void RegisterPluginModule() => AddPluginEventHandler<DefaultPluginEventHandler>();
}
