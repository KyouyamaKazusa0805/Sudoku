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
	AuthorGitHub,

	/// <summary>
	/// Indicates the author Gitee site.
	/// </summary>
	[Description("https://gitee.com/SunnieShine")]
	AuthorGitee,

	/// <summary>
	/// Indicates the author Bilibili site.
	/// </summary>
	[Description("https://space.bilibili.com/23736703")]
	Bilibili,

	/// <summary>
	/// Indicates the repository wiki page.
	/// </summary>
	[Description("https://sunnieshine.github.io/Sudoku/")]
	RepoWiki,

	/// <summary>
	/// Indicates the repository GitHub site.
	/// </summary>
	[Description("https://github.com/SunnieShine/Sudoku")]
	RepoGitHub,

	/// <summary>
	/// Indicates the repository Gitee site.
	/// </summary>
	[Description("https://gitee.com/SunnieShine/Sudoku")]
	RepoGitee,
}
