namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents a function page.
/// </summary>
public sealed partial class FunctionPage : Page
{
	/// <summary>
	/// Initializes a <see cref="FunctionPage"/> instance.
	/// </summary>
	public FunctionPage() => InitializeComponent();


	private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
	{
		// Check for local path. If the local contains at least one function file, we won't check for APIs.
		var existsValidFile = true;
		if (!Directory.Exists(CommonPaths.Functions))
		{
			Directory.CreateDirectory(CommonPaths.Functions); // Implictly create directory.
			existsValidFile = false;
		}

		var directoryInfo = new DirectoryInfo(CommonPaths.Functions);
		if (!directoryInfo.EnumerateFiles().Any())
		{
			existsValidFile = false;
		}

		// Export functions that marks '[ExportedMember]' if such function files don't exist.
		if (!existsValidFile)
		{
			foreach (var (propertyName, functionString) in getMetaProperties())
			{
				var fileName = getAvailableFileName(propertyName);
				var filePath = $@"{CommonPaths.Functions}\{fileName}{FileExtensions.UserFunction}";
				await File.WriteAllTextAsync(filePath, functionString);
			}
		}

		// Load functions from local files.
		foreach (var fileInfo in directoryInfo.EnumerateFiles())
		{
			await using var document = new ScriptDocument(fileInfo.FullName);
			if (!await document.LoadAsync())
			{
				continue;
			}

			var control = getFunctionInfoDisplayer(document);
			if (control is null)
			{
				continue;
			}

			FunctionsDisplayer.Children.Add(control);
			await Task.Delay(100);
		}


		static string getAvailableFileName(string propertyName)
		{
			var propertyNameUpdated = (stackalloc char[propertyName.Length]);
			propertyName.CopyTo(propertyNameUpdated);

			// Escape the character.
			foreach (ref var character in propertyNameUpdated)
			{
				if (Array.IndexOf(io::Path.GetInvalidFileNameChars(), character) != -1)
				{
					character = '_';
				}
			}

			// Then avoid confliction.
			if (io::Path.Exists($@"{CommonPaths.Functions}\{propertyNameUpdated}{FileExtensions.UserFunction}"))
			{
				for (var index = 1U; index <= uint.MaxValue; index++)
				{
					var targetFileName = $"{propertyNameUpdated}{index}";
					if (!io::Path.Exists($@"{CommonPaths.Functions}\{targetFileName}{FileExtensions.UserFunction}"))
					{
						return targetFileName;
					}
				}

				throw new("What the hell that the folder contains 4.2 billion files?!");
			}

			return propertyNameUpdated.ToString();
		}

		static SettingsCard? getFunctionInfoDisplayer(ScriptDocument document)
		{
			if (document.Method is not { } method)
			{
				return null;
			}

			var name = method.Name;
			var parameterTypes = from parameter in method.GetParameters() select parameter.ParameterType;
			var returnType = method.ReturnType;
			var parameterNamesString = string.Join(
				", ",
				from type in parameterTypes
				let typeForTechniqueArrayArray = type == typeof(Technique[][])
					? $"{TypeSystem.GetFriendlyTypeName(typeof(Technique), App.CurrentCulture)}[][]"
					: null
				select typeForTechniqueArrayArray ?? TypeSystem.GetFriendlyTypeName(type, App.CurrentCulture)
			);
			return new()
			{
				HeaderIcon = new FontIcon { Glyph = "\uE943" },
				Header = new TextBlock { Text = name, FontFamily = new("Cascadia Code") },
				Description = $"({parameterNamesString}) => {TypeSystem.GetFriendlyTypeName(returnType, App.CurrentCulture)}"
			};
		}

		static IEnumerable<(string PropertyName, string FunctionSyntaxString)> getMetaProperties()
		{
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static;
			foreach (var type in typeof(Analyzer).Assembly.GetTypes())
			{
				if (!type.IsAssignableTo(typeof(IFunctionProvider)))
				{
					continue;
				}

				foreach (var propertyInfo in type.GetProperties(bindingFlags))
				{
					if (propertyInfo.PropertyType != typeof(string))
					{
						continue;
					}

					if (!propertyInfo.IsDefined(typeof(ExportedMemberAttribute)))
					{
						continue;
					}

					var match = PropertyNamePattern().Match(propertyInfo.Name);
					if (!match.Success)
					{
						continue;
					}

					yield return (match.Groups[1].Value, (string)propertyInfo.GetValue(null)!);
				}
			}
		}
	}


	[GeneratedRegex("""_{2}(\w+)_RawSyntaxText""", RegexOptions.Compiled)]
	private static partial Regex PropertyNamePattern();
}
