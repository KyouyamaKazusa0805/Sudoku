#undef VISIT_SITE_DIRECTLY

namespace Sudoku.CommandLine.CommandOptions;

/// <summary>
/// Represents a visit command.
/// </summary>
public sealed class Visit : IRootCommand
{
	/// <summary>
	/// Indicates the link to visit.
	/// </summary>
	[Command('l', "link", "Indicates the link that outputs.")]
	[CommandConverter(typeof(EnumTypeConverter<VisitLink>))]
	public VisitLink VisitLink { get; set; } = VisitLink.AuthorGitHub;

	/// <inheritdoc/>
	public static string Name => "visit";

	/// <inheritdoc/>
	public static string Description => "To fetch the author or the repository link.";

	/// <inheritdoc/>
	public static string[] SupportedCommands => new[] { "visit" };

	/// <inheritdoc/>
	public static IEnumerable<(string CommandLine, string Meaning)>? UsageCommands =>
		new[]
		{
			(
				"""
				visit -l "author github"
				""",
				"Visits the GitHub link of the author."
			)
		};


	/// <inheritdoc/>
	public void Execute()
	{
		var attribute = typeof(Visit).GetProperty(nameof(VisitLink))!.GetCustomAttribute<DescriptionAttribute>()!;
		string link = attribute.Description;
		if (!Uri.TryCreate(link, UriKind.Absolute, out _))
		{
			throw new CommandLineRuntimeException((int)ErrorCode.SiteLinkIsInvalid);
		}

#if VISIT_SITE_DIRECTLY
		try
		{
			// Directly visit the site.
			Process.Start(
#if NET5_0_OR_GREATER
				new ProcessStartInfo(targetUri.AbsoluteUri) { UseShellExecute = true }
#else
				new ProcessStartInfo(targetUri.AbsoluteUri)
#endif
			);
		}
		catch
		{
			throw new CommandLineException((int)ErrorCode.SiteIsFailedToVisit);
		}
#else
		// Output the site link.
		Terminal.WriteLine(
			$"""
			Please visit the following site to learn more information.
			{link}
			"""
		);
#endif
	}
}
