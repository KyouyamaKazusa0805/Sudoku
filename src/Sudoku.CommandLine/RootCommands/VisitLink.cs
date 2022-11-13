namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Indicates the visit link.
/// </summary>
public enum VisitLink
{
	/// <summary>
	/// Indicates the author GitHub site.
	/// </summary>
	[SupportedArguments("author-github", "github", "g")]
	[Website("https://github.com/SunnieShine")]
	AuthorGitHub,

	/// <summary>
	/// Indicates the author Gitee site.
	/// </summary>
	[SupportedArguments("author-gitee", "gitee")]
	[Website("https://gitee.com/SunnieShine")]
	AuthorGitee,

	/// <summary>
	/// Indicates the author Bilibili site.
	/// </summary>
	[SupportedArguments("bilibili", "b")]
	[Website("https://space.bilibili.com/23736703")]
	Bilibili,

	/// <summary>
	/// Indicates the repository wiki page.
	/// </summary>
	[SupportedArguments("repo-wiki", "wiki", "w")]
	[Website("https://sunnieshine.github.io/Sudoku/")]
	RepoWiki,

	/// <summary>
	/// Indicates the repository GitHub site.
	/// </summary>
	[SupportedArguments("repo-github")]
	[Website("https://github.com/SunnieShine/Sudoku")]
	RepoGitHub,

	/// <summary>
	/// Indicates the repository Gitee site.
	/// </summary>
	[SupportedArguments("repo-gitee")]
	[Website("https://gitee.com/SunnieShine/Sudoku")]
	RepoGitee,
}
