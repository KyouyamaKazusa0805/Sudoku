#nullable disable

namespace Sudoku.CommandLine.Commands;

/// <summary>
/// Introduces the options that are used for visiting the author's profile and the program's site.
/// </summary>
[Verb("visit", HelpText = "To display the gathered websites that are about the author himself and the program.")]
public sealed class VisitOptions : IRootCommand
{
	/// <summary>
	/// To visit the author's GitHub site.
	/// </summary>
	[Option('g', "github", HelpText = "Indicates the author's GitHub page.", SetName = "page-github")]
	[Description("https://github.com/SunnieShine")]
	public bool AuthorGitHub { get; set; }

	/// <summary>
	/// To visit the author's Bilibili site.
	/// </summary>
	[Option('b', "bilibili", HelpText = "Indicates the author's Bilibili page.", SetName = "page-bilibili")]
	[Description("https://space.bilibili.com/23736703")]
	public bool Bilibili { get; set; }

	/// <summary>
	/// To visit the author's Gitee site.
	/// </summary>
	[Option("gitee", HelpText = "Indicates the author's Gitee page.", SetName = "page-gitee")]
	[Description("https://gitee.com/SunnieShine")]
	public bool AuthorGitee { get; set; }

	/// <summary>
	/// To visit the repo's wiki page.
	/// </summary>
	[Option('w', "repo-wiki", HelpText = "Indicates the repo's wiki page.", SetName = "page-repo-wiki")]
	[Description("https://sunnieshine.github.io/Sudoku/")]
	public bool RepoWiki { get; set; }

	/// <summary>
	/// To visit the repo's GitHub site.
	/// </summary>
	[Option('r', "repo", HelpText = "Indicates the repo's GitHub page.", SetName = "page-repo-github")]
	[Description("https://github.com/SunnieShine/Sudoku")]
	public bool RepoGitHub { get; set; }

	/// <summary>
	/// To visit the repo's Gitee site.
	/// </summary>
	[Option("repo-gitee", HelpText = "Indicates the repo's Gitee page.", SetName = "page-repo-gitee")]
	[Description("https://gitee.com/SunnieShine/Sudoku")]
	public bool RepoGitee { get; set; }


	/// <inheritdoc/>
	[Usage(ApplicationAlias = "Sudoku.CommandLine.exe")]
	public static IEnumerable<Example> Examples
	{
		get
		{
			yield return new(
				"Gets the link for the Bilibili page.",
				UnParserSettings.WithGroupSwitchesOnly(),
				new VisitOptions { Bilibili = true }
			);
			yield return new(
				"Gets the wiki page of the current sudoku repo.",
				UnParserSettings.WithGroupSwitchesOnly(),
				new VisitOptions { RepoWiki = true }
			);
		}
	}
}
