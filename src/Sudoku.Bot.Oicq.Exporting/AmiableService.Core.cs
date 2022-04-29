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
	public static List<IPluginEvent> Events = new();


	/// <include file='../../global-doc-comments.xml' path='g/static-constructor' />
	static AmiableService()
	{
		AppDomain.CurrentDomain.UnhandledException += static (_, e) =>
		{
			object exceptionObject = e.ExceptionObject;
			App.Log("[AppDomainError]", exceptionObject.ToString());
		};

		// Initializes the fields and properties.
		App = new AppService();
		SetAppInfo();
		ServiceBuilder(App);
		RegisterPluginModule();

		// Invokes the event.
		EventCore.InvokeEvents(AmiableEventType.AmiableLoaded, new());
		App.Log("[AppDomain]", AppDomain.CurrentDomain.FriendlyName);
	}


	/// <summary>
	/// Adds the specified <see cref="IPluginEvent"/> instance into the event handler collection.
	/// </summary>
	/// <typeparam name="TPluginEvent">The type of the plugin.</typeparam>
	public static void AddPluginEvent<TPluginEvent>() where TPluginEvent : IPluginEvent, new()
		=> Events.Add((IPluginEvent)Activator.CreateInstance(typeof(TPluginEvent)));
}
