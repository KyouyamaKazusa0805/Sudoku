namespace Sudoku.Diagnostics.CodeGen.Generators;

/// <summary>
/// Defines the global configuration value source generator.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class GlobalConfigValueGenerator : ISourceGenerator
{
	/// <inheritdoc/>
	public void Execute(GeneratorExecutionContext context)
	{
		if (context.AdditionalFiles is not { IsDefaultOrEmpty: false } additionalFiles)
		{
			return;
		}

		var additionalFile = additionalFiles.FirstOrDefault(globalConfigurationFileChecker);
		if (additionalFile is not { Path: var path })
		{
			return;
		}

		var xmlDocument = new XmlDocument().OnLoading(path);
		if (xmlDocument is not { DocumentElement: { } root })
		{
			return;
		}

		if (root.SelectNodes("descendant::PropertyGroup") is not { } propertyGroupList)
		{
			return;
		}

		if (propertyGroupList.Cast<XmlNode>().FirstOrDefault() is not { ChildNodes: [_, ..] elements })
		{
			return;
		}

		Version? versionResult = null;
		bool found = false;
		foreach (object element in elements)
		{
			if (element is XmlNode { Name: "Version", InnerText: var v }
				&& Version.TryParse(v, out versionResult))
			{
				found = true;
				break;
			}
		}
		if (!found)
		{
			return;
		}

		context.AddSource(
			"Constants.Version.g.cs",
			$@"namespace Sudoku.Diagnostics.CodeGen;

partial class Constants
{{
	/// <summary>
	/// Indicates the version of this project.
	/// </summary>
	public const string VersionValue = ""{versionResult!}"";
}}"
		);


		static bool globalConfigurationFileChecker(AdditionalText additionalText)
		{
			const string comparer = "Directory.Build.props";
			return additionalText.Path is var path && (path == comparer || path.EndsWith(comparer));
		}
	}

	/// <inheritdoc/>
	public void Initialize(GeneratorInitializationContext context)
	{
	}
}
