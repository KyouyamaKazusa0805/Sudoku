#undef VISIT_SITE_DIRECTLY

namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a visit command.
/// </summary>
[RootCommand("visit", DescriptionResourceKey = "_Description_Visit")]
[SupportedArguments("visit")]
[Usage("visit -l <link>", IsPattern = true)]
[Usage("visit -l author-github", DescriptionResourceKey = "_Usage_Visit_1")]
public sealed class Visit : IExecutable
{
	/// <summary>
	/// Indicates the link to visit.
	/// </summary>
	[DoubleArgumentsCommand('l', "link", DescriptionResourceKey = "_Description_VisitLink_Visit")]
	[CommandConverter<EnumTypeConverter<VisitLink>>]
	public VisitLink VisitLink { get; set; } = VisitLink.AuthorGitHub;


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
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
		await Terminal.WriteLineAsync($"{R["_MessageFormat_PleaseVisitFollowingLink"]!}\r\n{link.AbsoluteUri}");
#endif
	}
}
