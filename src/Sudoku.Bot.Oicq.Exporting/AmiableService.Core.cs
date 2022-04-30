#nullable enable

namespace Sudoku.Bot.Oicq.Exporting;

partial class AmiableService
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
	/// Adds the specified <see cref="IPluginEventHandler"/> instance into the event handler collection.
	/// </summary>
	/// <typeparam name="TPluginEventHandler">The type of the plugin event handler.</typeparam>
	public static void AddPluginEvent<TPluginEventHandler>() where TPluginEventHandler : IPluginEventHandler, new()
		=> EventHandlers.Add(Activator.CreateInstance<TPluginEventHandler>());
}
