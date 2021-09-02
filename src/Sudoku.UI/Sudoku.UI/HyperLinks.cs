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
	/// Indicates the website that is the series of video-formed sudoku tutorial.
	/// </summary>
	public static readonly Uri VideoTutorial = new(VideoTutorialLink);

	/// <summary>
	/// Indicates the website that is the repository site on GitHub.
	/// </summary>
	public static readonly Uri Repo = new(RepoLink);

	/// <summary>
	/// Indicates the website that is the project wiki related to the repository on GitHub.
	/// </summary>
	public static readonly Uri ProjectWiki = new(ProjectWikiLink);
}
