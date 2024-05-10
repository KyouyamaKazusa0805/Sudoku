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
	/// Indicates the separator.
	/// </summary>
	private const string Separator = " | ";


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
		IgnoreReadOnlyProperties = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new json::CultureInfoConverter(), new json::DateOnlyConverter("yyyy/MM/dd") }
	};

	/// <summary>
	/// Indicates the UTF-8 encoding.
	/// </summary>
	private static readonly Encoding Utf8Encoding = Encoding.UTF8;


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
			ErrorDisplayer.Text = string.Empty;
			IsActiveProgressRing.IsActive = true;
			var downloadText = ResourceDictionary.Get("OnlinePuzzleLibraryPage_DownloadLibrary", App.CurrentCulture);

			// Fetch libraries and their own information.
			var librariesAndInfos = await _gitHubClient.Repository.Content.GetAllContents(MyName, RepositoryName, "libraries");
			foreach (var d1 in librariesAndInfos)
			{
				foreach (var d2 in librariesAndInfos)
				{
					if (d1.Path == d2.Path)
					{
						// Same file.
						continue;
					}

					var (name1, name2) = (d1.Name, d2.Name);
					var extension1 = io::Path.GetExtension(name1);
					var extension2 = io::Path.GetExtension(name2);
					var fileName1 = io::Path.GetFileNameWithoutExtension(name1);
					var fileName2 = io::Path.GetFileNameWithoutExtension(name2);
					if (extension1 != FileExtensions.PlainText || extension2 != FileExtensions.JsonDocument
						|| fileName1 != fileName2)
					{
						// Fix the appearance order.
						continue;
					}

					var (libraryFile, jsonFile) = (d1, d2);
					var fileUrl = jsonFile.DownloadUrl;
					await using var stream = new MemoryStream();
					if (await _httpClient.DownloadFileAsync(fileUrl, stream) is { } exceptionThrown)
					{
						// TODO: Load exception message to text block or throw it.
						continue;
					}

					var contentJson = Utf8Encoding.GetString(stream.ToArray());
					var libraryInfo = JsonSerializer.Deserialize<OnlineLibraryInfo>(contentJson, Options);
					if (libraryInfo is not { Data: { } libraryData })
					{
						// Data is invalid.
						continue;
					}

					var libDetailCurrentCulture = libraryData.FirstOrDefault(cultureMatch);
					if (libDetailCurrentCulture is not { Name: { } name })
					{
						// Name or other values is valid.
						continue;
					}

					var lastSlashPos = fileUrl.LastIndexOf('/');
					var fileIdWithExtension = fileUrl[(lastSlashPos + 1)..];
					var fileId = fileIdWithExtension[fileIdWithExtension.LastIndexOf('.') is var p and not -1 ? ..p : ..];
					var downloadButton = new Button { Content = downloadText };
					var progressRing = new ProgressRing
					{
						HorizontalAlignment = HorizontalAlignment.Center,
						Visibility = Visibility.Collapsed
					};
					downloadButton.Click += async (_, _) =>
					{
						downloadButton.IsEnabled = false;
						progressRing.Visibility = Visibility.Visible;

						await downloadButtonHandler(name, fileId, libraryInfo, libDetailCurrentCulture);

						downloadButton.IsEnabled = true;
						progressRing.Visibility = Visibility.Collapsed;
					};
					PuzzleLibrariesOnGitHubDisplayer.Children.Add(
						new SettingsCard
						{
							Header = name,
							HeaderIcon = new FontIcon { Glyph = "\uE8F1" },
							Description = string.Join(
								Separator,
								[
									fileId,
									libraryInfo.Author,
									libraryInfo.LastUpdate.ToString(App.CurrentCulture)
								]
							),
							Content = new StackPanel
							{
								Orientation = Orientation.Horizontal,
								Spacing = 6,
								Children = { downloadButton, progressRing }
							}
						}
					);

					_files.Add(libraryInfo);
					break; // A JSON file must be matched with only one library file.
				}
			}
		}
		catch (RateLimitExceededException ex)
		{
			var formatMessage = ResourceDictionary.Get("ExceptionMessage_ExceedRateLimit", App.CurrentCulture);
			ErrorDisplayer.Text = string.Format(formatMessage, ex.Reset.LocalDateTime.ToString());
		}
		catch (OperationCanceledException)
		{
			// Do nothing.
		}
		catch (JsonException)
		{
			throw;
		}
		catch (HttpRequestException ex)
		{
			ErrorDisplayer.Text = ex.HttpRequestError.ToString();
		}
		catch (Exception ex)
		{
			ErrorDisplayer.Text = ex.Message;
		}
		finally
		{
			IsActiveProgressRing.IsActive = false;
			IsActiveProgressRing.Visibility = Visibility.Collapsed;
		}


		static bool cultureMatch(OnlineLibraryDetail data) => data.Culture?.Equals(App.CurrentCulture) ?? false;

		async Task downloadButtonHandler(string name, string fileId, OnlineLibraryInfo libraryInfo, OnlineLibraryDetail libDataChosen)
		{
			try
			{
				var filePath = await loadPuzzleOnlineAsync(name, fileId);
				writeConfigFileAlso(filePath, libraryInfo, libDataChosen);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
		}

		async Task<string> loadPuzzleOnlineAsync(string libraryName, string fileId)
		{
			const string prefix = "https://raw.githubusercontent.com/SunnieShine/sudoku-puzzle-libraries/main/libraries/";
			CommonPaths.CreateIfNotExist(CommonPaths.Cache);

			var fileIdWithExtension = $@"{fileId}{FileExtensions.PlainText}";
			var cachedFilePath = $@"{CommonPaths.Cache}\{fileIdWithExtension}";
			try
			{
				await _httpClient.DownloadFileAsync($"{prefix}{fileIdWithExtension}", cachedFilePath, _cts.Token);
				File.Move(cachedFilePath, cachedFilePath = $@"{CommonPaths.Library}\{fileIdWithExtension}");
				return cachedFilePath;
			}
			catch (TaskCanceledException)
			{
				throw;
			}
		}

		static void writeConfigFileAlso(string cachedFilePath, OnlineLibraryInfo libraryInfo, OnlineLibraryDetail libDataChosen)
		{
			var folder = io::Path.GetDirectoryName(cachedFilePath);
			var fileName = io::Path.GetFileNameWithoutExtension(cachedFilePath);
			var newFilePath = $@"{folder}\{fileName}{FileExtensions.JsonDocument}";
			var library = new Library(CommonPaths.Library, fileName);
			library.Initialize();
			library.Name = libDataChosen.Name;
			library.Author = libraryInfo.Author;
			library.Description = libDataChosen.Description;
			if (libDataChosen.Tags.Length != 0)
			{
				library.Tags = libDataChosen.Tags;
			}
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
