namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// Provides with the Amiable service as the base service. The type will be used automatically.
/// </summary>
public static partial class AmiableService
{
	/// <summary>
	/// Sets the app info.
	/// </summary>
	public static void SetAppInfo()
		=> App.AppInfo = new("Sudoku.Bot.Oicq.Exporting", "0.1.0", "SunnieShine", "测试机器人程序", "app.sunnie.demo");

	/// <summary>
	/// Builds the services and make compatible platform configurations.
	/// </summary>
	/// <param name="service">The current service.</param>
	public static void ServiceBuilder(AppService service) => service.AddMQConfig();

	/// <summary>
	/// Registers the plugin module.
	/// </summary>
	private static void RegisterPluginModule() => AddPluginEvent<TestPluginEvent>();
}
