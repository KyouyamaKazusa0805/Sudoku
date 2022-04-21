namespace Sudoku.CommandLine;

/// <summary>
/// Indicates the visit link.
/// </summary>
public enum VisitLink
{
	/// <summary>
	/// Indicates the author GitHub site.
	/// </summary>
	[Description("https://github.com/SunnieShine")]
	[SupportedNames(new[] { "author-github", "github", "g" })]
	AuthorGitHub,

	/// <summary>
	/// Indicates the author Gitee site.
	/// </summary>
	[Description("https://gitee.com/SunnieShine")]
	[SupportedNames(new[] { "author-gitee", "gitee" })]
	AuthorGitee,

	/// <summary>
	/// Indicates the author Bilibili site.
	/// </summary>
	[Description("https://space.bilibili.com/23736703")]
	[SupportedNames(new[] { "bilibili", "b" })]
	Bilibili,

	/// <summary>
	/// Indicates the repository wiki page.
	/// </summary>
	[Description("https://sunnieshine.github.io/Sudoku/")]
	[SupportedNames(new[] { "repo-wiki", "wiki", "w" })]
	RepoWiki,

	/// <summary>
	/// Indicates the repository GitHub site.
	/// </summary>
	[Description("https://github.com/SunnieShine/Sudoku")]
	[SupportedNames(new[] { "repo-github" })]
	RepoGitHub,

	/// <summary>
	/// Indicates the repository Gitee site.
	/// </summary>
	[Description("https://gitee.com/SunnieShine/Sudoku")]
	[SupportedNames(new[] { "repo-gitee" })]
	RepoGitee,
}
