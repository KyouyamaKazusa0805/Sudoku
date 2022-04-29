namespace Sudoku.Bot.Oicq.Adapter;

/// <summary>
/// Provides with methods that adds the Kum SDK configuration into the <see cref="AppService"/> instance.
/// </summary>
/// <seealso cref="AppService"/>
public static class MqAppServiceExtension
{
	/// <summary>
	/// Provides with the API wrapper for MyQQ SDK.
	/// </summary>
	/// <param name="this">The <see cref="AppService"/> instance.</param>
	/// <returns>The reference that is same as <paramref name="this"/>.</returns>
	public static AppService AddMQConfig(this AppService service)
	{
		service.AddAppInfoConverter<MqAppInfoConverter>("MQ");
		service.ApiWrappers.Add("MQ", new MqApiWrapper());
		service.CodeProviders.Add("MQ", new IrCodeProvider());
		return service;
	}
}
