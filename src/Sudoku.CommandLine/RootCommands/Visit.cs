#undef VISIT_SITE_DIRECTLY

namespace Sudoku.CommandLine.RootCommands;

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
		var attribute = typeof(Visit).GetProperty(nameof(VisitLink))!.GetCustomAttribute<WebsiteAttribute>()!;
		var link = attribute.Site;

#if VISIT_SITE_DIRECTLY
		try
		{
			// Directly visit the site.
			System.Diagnostics.Process.Start(
#if NET5_0_OR_GREATER
				new System.Diagnostics.ProcessStartInfo(link.AbsoluteUri) { UseShellExecute = true }
#else
				new System.Diagnostics.ProcessStartInfo(link.AbsoluteUri)
#endif
			);
		}
		catch
		{
			throw new CommandLineRuntimeException((int)ErrorCode.SiteIsFailedToVisit);
		}
#else
		// Output the site link.
		Terminal.WriteLine(
			$"""
			Please visit the following site to learn more information.
			{link.AbsoluteUri}
			"""
		);
#endif
	}
}
