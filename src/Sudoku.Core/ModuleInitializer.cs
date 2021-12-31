#define STRICT_RESOURCE_FILE_PATH_CHECKING
#define USE_CALLING_ASSEMBLY
#undef USE_EXECUTING_ASSEMBLY

namespace Sudoku;

/// <summary>
/// <para>Provides with the type that contains a module initializer.</para>
/// <para><b>
/// The type is called by the compiler and the CLR (Common Language Runtime),
/// which means you cannot use any members in this type manually.
/// </b></para>
/// </summary>
internal static class ModuleInitializer
{
	/// <summary>
	/// Indicates the pattern that matches the name of the resource file names.
	/// </summary>
	private static readonly Regex ResourceFileNamePattern = new(
		@"lang\-(default|[1-9]\d{3,4}|[A-Za-z]{2}(\-[A-Za-z]{2})?)\.json",
		RegexOptions.Compiled,
		TimeSpan.FromSeconds(5)
	);


	/// <summary>
	/// Initializes the current module.
	/// </summary>
	/// <exception cref="DirectoryNotFoundException">
	/// Throws when the language resource folder cannot be found.
	/// </exception>
	/// <exception cref="FileNotFoundException">
	/// Throws when the language resource file cannot be found.
	/// </exception>
	/// <exception cref="InvalidOperationException">
	/// <para>
	/// Throws when the found resource file doesn't satisfy some condition, which leads to a failure
	/// to parse and convert it into a valid <see cref="ResourceDocument"/> instance.
	/// </para>
	/// <para>
	/// In addition, the exception will be thrown if the compilation symbol
	/// <c>STRICT_RESOURCE_FILE_PATH_CHECKING</c> is enabled; if failed to parse without the symbol,
	/// the file will be skipped directly instead.
	/// </para>
	/// </exception>
	[ModuleInitializer]
	public static void Initialize()
	{
#if USE_CALLING_ASSEMBLY
		string directory = Assembly.GetCallingAssembly().Location;
#elif USE_EXECUTING_ASSEMBLY
		string directory = Assembly.GetExecutingAssembly().Location;
#else
		string directory = null!;
#error You must set one symbol in either 'USE_CALLING_ASSEMBLY' or 'USE_EXECUTING_ASSEMBLY'.
#endif

		if (string.IsNullOrEmpty(directory))
		{
			throw new DirectoryNotFoundException("The language resource folder cannot be found.");
		}

		var targetDirectory = new DirectoryInfo($@"{directory}\lang");
		if (!targetDirectory.Exists)
		{
			throw new DirectoryNotFoundException("The language resource folder cannot be found.");
		}

		var files = targetDirectory.GetFiles("*.json", SearchOption.TopDirectoryOnly);
		if (files.Length == 0)
		{
			throw new FileNotFoundException("The resource document file cannot be found.");
		}

		var targetManager = new ResourceDocumentManager();
		foreach (var file in files)
		{
			if (file is not { Name: var name, FullName: var fullPath })
			{
				continue;
			}

			try
			{
				if (!ResourceFileNamePattern.IsMatch(name))
				{
#if STRICT_RESOURCE_FILE_PATH_CHECKING
					throw new InvalidOperationException(
						$"The file name is invalid. For more information about the naming rules of the resource file, please visit the field '{nameof(ResourceFileNamePattern)}' in this type."
					);
#else
					continue;
#endif
				}
			}
			catch (
				RegexMatchTimeoutException
#if STRICT_RESOURCE_FILE_PATH_CHECKING
					ex
#endif
			)
			{
#if STRICT_RESOURCE_FILE_PATH_CHECKING
				throw new InvalidOperationException(
					$"The file name is failed to match with the naming rules. For more information about the naming of the resource file, please visit the field '{nameof(ResourceFileNamePattern)}' in this type.",
					ex
				);
#else
				continue;
#endif
			}

			string json = File.ReadAllText(fullPath);
			if (!ResourceDocument.TryParse(json, out var result))
			{
#if STRICT_RESOURCE_FILE_PATH_CHECKING
				throw new InvalidOperationException("The file cannot be parsed to a valid target document instance.");
#else
				continue;
#endif
			}

			targetManager.Add(result);
		}

		ResourceDocumentManager.Shared = targetManager;
	}
}
