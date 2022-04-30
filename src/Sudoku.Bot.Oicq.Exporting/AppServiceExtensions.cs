namespace Sudoku.Bot.Oicq.Exporting;

/// <summary>
/// Provides with methods that adds the Kum SDK configuration into the <see cref="AppService"/> instance.
/// </summary>
/// <seealso cref="AppService"/>
public static class AppServiceExtensions
{
	/// <summary>
	/// Provides with the API wrapper for MyQQ SDK.
	/// </summary>
	/// <param name="this">The <see cref="AppService"/> instance.</param>
	/// <returns>The reference that is same as <paramref name="this"/>.</returns>
	public static AppService AddConfig(this AppService service)
	{
		service.AddAppInfoConverter<AppInfoConverter>("MQ");
		service.ApiWrappers.Add("MQ", new ApiWrapper());
		service.CodeProviders.Add("MQ", new IrCodeProvider());
		return service;
	}
}
