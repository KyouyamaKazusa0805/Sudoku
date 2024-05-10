namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a page that can download online puzzle libraries from my own repository
/// "<see href="https://github.com/SunnieShine/sudoku-puzzle-libraries">Sudoku Puzzle Libraries</see>".
/// </summary>
/// <!--https://raw.githubusercontent.com/SunnieShine/sudoku-puzzle-libraries/main/libraries/-->
public sealed partial class OnlinePuzzleLibraryPage : Page
{
	/// <summary>
	/// Indicates the repository name.
	/// </summary>
	private const string RepositoryName = "sudoku-puzzle-libraries";

	/// <summary>
	/// Indicates my name.
	/// </summary>
	private const string MyName = "SunnieShine";


	/// <summary>
	/// Indicates the URI for libraries repo.
	/// </summary>
	private static readonly Uri RepoUri = new("https://github.com/SunnieShine/sudoku-puzzle-libraries");

	/// <summary>
	/// Indicates the backing serialzer options.
	/// </summary>
	private static readonly JsonSerializerOptions Options = new()
	{
		WriteIndented = true,
		Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
		IncludeFields = false,
		IgnoreReadOnlyProperties = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new CultureInfoConverter() }
	};


	/// <summary>
	/// Indicates the files in the repository.
	/// </summary>
	private readonly List<OnlineLibraryInfo> _files = [];

	/// <summary>
	/// Indicates the cancellation token source.
	/// </summary>
	private CancellationTokenSource _cts;

	/// <summary>
	/// Indicates the web client.
	/// </summary>
	private HttpClient _httpClient;

	/// <summary>
	/// Indicates the backing GitHub client.
	/// </summary>
	private GitHubClient _gitHubClient;


	/// <summary>
	/// Initializes an <see cref="OnlinePuzzleLibraryPage"/> instance.
	/// </summary>
	public OnlinePuzzleLibraryPage()
	{
		InitializeComponent();
		InitializeFields();
	}


	/// <summary>
	/// Initializes for fields.
	/// </summary>
	[MemberNotNull(nameof(_cts), nameof(_httpClient), nameof(_gitHubClient))]
	private void InitializeFields()
	{
		_cts = new();
		_httpClient = new();
		_gitHubClient = new(new ProductHeaderValue("Sudoku"), RepoUri);
	}


	private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
	{
		try
		{
			ErrorDisplayer.Text = "";
			IsActiveProgressRing.IsActive = true;
			var downloadText = ResourceDictionary.Get("OnlinePuzzleLibraryPage_DownloadLibrary", App.CurrentCulture);

			await using var stream = new MemoryStream();

			// Fetch libraries and their own information.
			var librariesAndInfos = await _gitHubClient.Repository.Content.GetAllContents(MyName, RepositoryName, "libraries");
			foreach (var libraryInfo in
				from d1 in librariesAndInfos
				from d2 in librariesAndInfos
				where d1.Path != d2.Path
				let pair = (Name1: d1.Name, Name2: d2.Name)
				let extension1 = io::Path.GetExtension(pair.Name1)
				let extension2 = io::Path.GetExtension(pair.Name2)
				let fileName1 = io::Path.GetFileNameWithoutExtension(pair.Name1)
				let fileName2 = io::Path.GetFileNameWithoutExtension(pair.Name2)
				where extension1 == ".txt" && extension2 == ".json" && fileName1 == fileName2
				select (LibraryFile: d1, JsonFile: d2) into filePair
				let fileUrl = filePair.JsonFile.DownloadUrl
				let exceptionThrown = _httpClient.DownloadFileAsync(fileUrl, stream)
				where exceptionThrown is null
				let contentJson = Encoding.UTF8.GetString(stream.ToArray())
				let instance = JsonSerializer.Deserialize<OnlineLibraryInfo>(contentJson, Options)
				where instance is { Data: not null }
				select instance)
			{
				var libData = libraryInfo.Data!;
				var libDataChosen = libData.FirstOrDefault(static data => data.Culture?.Equals(App.CurrentCulture) ?? false);
				if (libDataChosen is null)
				{
					continue;
				}

				var author = libraryInfo.Author;
				var name = libDataChosen.Name!;
				var lastUpdate = libraryInfo.LastUpdate;

				var downloadButton = new Button { Content = downloadText };
				downloadButton.Click += async (_, _) =>
				{
					try
					{
						await _httpClient.DownloadFileAsync(
							$"https://raw.githubusercontent.com/SunnieShine/sudoku-puzzle-libraries/main/libraries/{name}.txt",
							$@"{CommonPaths.Library}\{name}.txt",
							_cts.Token
						);
					}
					catch (OperationCanceledException)
					{
					}
				};

				PuzzleLibrariesOnGitHubDisplayer.Children.Add(
					new SettingsCard
					{
						Header = name,
						HeaderIcon = new FontIcon { Glyph = "\uE8F1" },
						Description = string.Join(" | ", [author, lastUpdate.ToString(App.CurrentCulture)]),
						Content = downloadButton
					}
				);

				_files.Add(libraryInfo);
			}
		}
		catch (RateLimitExceededException ex)
		{
			ErrorDisplayer.Text = string.Join(
				ResourceDictionary.Get("ExceptionMessage_ExceedRateLimit", App.CurrentCulture),
				ex.Reset.LocalDateTime.ToString()
			);
			return;
		}
		finally
		{
			IsActiveProgressRing.IsActive = false;
			IsActiveProgressRing.Visibility = Visibility.Collapsed;
		}
	}

	private void Page_Unloaded(object sender, RoutedEventArgs e)
	{
		_cts.Cancel();
		_cts.Dispose();
		_httpClient.Dispose();
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <inheritdoc cref="DownloadFileAsync(HttpClient, string, string, CancellationToken)"/>
	public static async Task<Exception?> DownloadFileAsync(this HttpClient @this, string url, Stream stream, CancellationToken cancellationToken = default)
	{
		try
		{
			var byteData = await @this.GetByteArrayAsync(url, cancellationToken);
			await stream.WriteAsync(byteData, cancellationToken);
			return null;
		}
		catch (Exception ex)
		{
			return ex;
		}
	}

	/// <summary>
	/// Downloads a file from web.
	/// </summary>
	/// <param name="this">The client instance.</param>
	/// <param name="url">The specified URL.</param>
	/// <param name="path">The path where the downloaded data to be stored.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current task.</param>
	/// <returns>An asynchronous task instance that holds the task data to be called.</returns>
	public static async Task<Exception?> DownloadFileAsync(this HttpClient @this, string url, string path, CancellationToken cancellationToken = default)
	{
		if (File.Exists(path))
		{
			return new IOException(ResourceDictionary.ExceptionMessage("DownloadFileExists"));
		}

		try
		{
			var byteData = await @this.GetByteArrayAsync(url, cancellationToken);
			await using var responseStream = new MemoryStream(byteData);
			var downloadBuffer = new byte[(1 << 10) * 1000];
			int byteSize;
			using var fileStream = new FileStream(path, FileMode.CreateNew, FileAccess.Write);
			while ((byteSize = responseStream.Read(downloadBuffer, 0, downloadBuffer.Length)) > 0)
			{
				await fileStream.WriteAsync(downloadBuffer.AsMemory(0, byteSize), cancellationToken);
			}
			return null;
		}
		catch (Exception ex)
		{
			return ex;
		}
	}
}
