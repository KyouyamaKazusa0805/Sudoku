#undef VISIT_SITE_DIRECTLY

namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a visit command.
/// </summary>
[RootCommand("visit", "To fetch the author or the repository link.")]
[SupportedArguments(new[] { "visit" })]
[Usage("visit -l <link>", IsPattern = true)]
[Usage("""visit -l author-github""", "Visits the GitHub link of the author.")]
public sealed class Visit : IExecutable
{
	/// <summary>
	/// Indicates the link to visit.
	/// </summary>
	[DoubleArgumentsCommand('l', "link", "Indicates the link that outputs.")]
	[CommandConverter<EnumTypeConverter<VisitLink>>]
	public VisitLink VisitLink { get; set; } = VisitLink.AuthorGitHub;


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
