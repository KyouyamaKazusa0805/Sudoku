namespace Sudoku.Bot.Oicq;

/// <summary>
/// Defines an app service.
/// </summary>
public class AppService
{
	/// <summary>
	/// Indicates the backing field of the property <see cref="Instance"/>.
	/// </summary>
	/// <seealso cref="Instance"/>
	private static AppService? _instance;


	/// <summary>
	/// Indicates a set of <see cref="IAppInfoConverter"/>s.
	/// </summary>
	private readonly Dictionary<string, IAppInfoConverter> _appInfoConverters = new();


	/// <summary>
	/// Indicates the event filters.
	/// </summary>
	public List<IEventFilter> EventFilters = new();

	/// <summary>
	/// Defines a set of API wrappers.
	/// </summary>
	public Dictionary<string, IApiWrapper> ApiWrappers = new();

	/// <summary>
	/// Defines a set of code providers.
	/// </summary>
	public Dictionary<string, CodeProvider> CodeProviders = new();

	/// <summary>
	/// Indicates the key of the API.
	/// </summary>
	private string _apiKey = null!;

	/// <summary>
	/// Indicates the App information.
	/// </summary>
	public AppInfo AppInfo = null!;


	/// <summary>
	/// Initializes an <see cref="AppService"/> instance.
	/// </summary>
	public AppService() => _instance = this;


	/// <summary>
	/// Indicates the instance of the current type.
	/// </summary>
	public static AppService Instance => _instance ??= new AppService();



	/// <summary>
	/// Gets an API wrapper instance.
	/// </summary>
	public IApiWrapper DefaultApiWrapper
	{
		get
		{
			if (string.IsNullOrEmpty(_apiKey))
			{
				return ApiWrappers.First().Value;
			}

			if (ApiWrappers.ContainsKey(_apiKey) is false)
			{
				return ApiWrappers.First().Value;
			}

			return ApiWrappers[_apiKey];
		}
	}

	/// <summary>
	/// Gets the tooling instance for the code provider.
	/// </summary>
	public CodeProvider DefaultCodeProvider =>
		string.IsNullOrEmpty(_apiKey)
			? CodeProviders.First().Value
			: ApiWrappers.ContainsKey(_apiKey) ? CodeProviders[_apiKey] : CodeProviders.First().Value;


	/// <summary>
	/// Sets the API key with the speciifed value.
	/// </summary>
	/// <param name="key">The key.</param>
	public void SetApiKey(string key) => _apiKey = key;

	/// <summary>
	/// Adds the specified app info converter into the collection via the key.
	/// </summary>
	/// <typeparam name="TAppInfoConverter">The type of the app info converter.</typeparam>
	/// <param name="key">The key that corresponds to the app info converter.</param>
	public void AddAppInfoConverter<TAppInfoConverter>(string key) where TAppInfoConverter : IAppInfoConverter, new()
		=> _appInfoConverters.Add(key, new TAppInfoConverter());

	/// <summary>
	/// Logs the text via the arguments.
	/// </summary>
	/// <param name="args">The arguments.</param>
	public void Log(params object[] args)
	{
		string dir = Path.Combine(Directory.GetCurrentDirectory(), "Amiable");
		if (!Directory.Exists(dir))
		{
			Directory.CreateDirectory(dir);
		}

		string path = Path.Combine(dir, $"{AppInfo.Name ?? "Amiable"}.{DateTime.Now:yyMMdd}.log");
		File.AppendAllText(path, $"[{DateTime.Now.ToShortTimeString()}]{string.Join(string.Empty, args)}\n");
	}

	/// <summary>
	/// Logs for the debug text via the specified arguments.
	/// </summary>
	/// <param name="args">The arguments.</param>
	public void Debug(params object[] args) => Log("[Debug]", args);

	/// <summary>
	/// Gets the string representation of an app info instance.
	/// </summary>
	/// <returns>The string representation of an app info instance.</returns>
	public string GetAppInfoSring() => _appInfoConverters[_apiKey].Convert(AppInfo);
}
