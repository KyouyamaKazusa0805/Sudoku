namespace Sudoku.UI;

/// <summary>
/// Provides with the links that references and links to a website.
/// </summary>
public static class HyperLinks
{
	/// <summary>
	/// Indicates the direct link that referenced by <see cref="VideoTutorial"/>.
	/// </summary>
	/// <seealso cref="VideoTutorial"/>
	public const string VideoTutorialLink = "https://www.bilibili.com/video/BV1Mx411z7uq";

	/// <summary>
	/// Indicates the direct link that referenced by <see cref="Repo"/>.
	/// </summary>
	/// <seealso cref="Repo"/>
	public const string RepoLink = "https://github.com/SunnieShine/Sudoku";

	/// <summary>
	/// Indicates the direct link that referenced by <see cref="ProjectWiki"/>.
	/// </summary>
	/// <seealso cref="ProjectWiki"/>
	public const string ProjectWikiLink = "https://sunnieshine.github.io/Sudoku";

	/// <summary>
	/// Indicates the direct link that referenced by <see cref="RepoGitee"/>.
	/// </summary>
	/// <seealso cref="RepoGitee"/>
	public const string RepoGiteeLink = "https://gitee.com/SunnieShine/Sudoku";

	/// <summary>
	/// Indicates the direct link that referenced by <see cref="SpecialThanks1"/>.
	/// </summary>
	/// <seealso cref="SpecialThanks1"/>
	public const string SpecialThanks1Link = "https://github.com/VoBilyk/SudokuSolverOCR";

	/// <summary>
	/// Indicates the direct link that referenced by <see cref="SpecialThanks2"/>.
	/// </summary>
	/// <seealso cref="SpecialThanks2"/>
	public const string SpecialThanks2Link = "https://github.com/dobrichev/fsss2";

	/// <summary>
	/// Indicates the direct link that referenced by <see cref="SpecialThanks3"/>.
	/// </summary>
	/// <seealso cref="SpecialThanks3"/>
	public const string SpecialThanks3Link = "https://sourceforge.net/projects/hodoku";

	/// <summary>
	/// Indicates the direct link that referenced by <see cref="SpecialThanks4"/>.
	/// </summary>
	/// <seealso cref="SpecialThanks4"/>
	public const string SpecialThanks4Link = "http://diuf.unifr.ch/pai/people/juillera/Sudoku/Sudoku.html";


	/// <summary>
	/// Indicates the website that is the series of video-formed sudoku tutorial.
	/// </summary>
	public static readonly Uri VideoTutorial = new(VideoTutorialLink);

	/// <summary>
	/// Indicates the website that is the GitHub repository site, which is the main repository site.
	/// </summary>
	public static readonly Uri Repo = new(RepoLink);

	/// <summary>
	/// Indicates the website that is the project wiki related to the repository on GitHub.
	/// </summary>
	public static readonly Uri ProjectWiki = new(ProjectWikiLink);

	/// <summary>
	/// Indicates the website that is the Gitee repository site.
	/// </summary>
	public static readonly Uri RepoGitee = new(RepoGiteeLink);

	/// <summary>
	/// Indicates the website that is the first site of the special thanks list.
	/// </summary>
	public static readonly Uri SpecialThanks1 = new(SpecialThanks1Link);

	/// <summary>
	/// Indicates the website that is the second site of the special thanks list.
	/// </summary>
	public static readonly Uri SpecialThanks2 = new(SpecialThanks2Link);

	/// <summary>
	/// Indicates the website that is the third site of the special thanks list.
	/// </summary>
	public static readonly Uri SpecialThanks3 = new(SpecialThanks3Link);

	/// <summary>
	/// Indicates the website that is the fourth site of the special thanks list.
	/// </summary>
	public static readonly Uri SpecialThanks4 = new(SpecialThanks4Link);
}
