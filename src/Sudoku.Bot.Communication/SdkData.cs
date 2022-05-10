namespace Sudoku.Bot.Communication;

/// <summary>
/// Indicates the encapsulation of the SDK information, corresponds to the original author.
/// </summary>
internal static class SdkData
{
	/// <summary>
	/// The repository of the current open-source product.
	/// </summary>
	public const string RepoLink_Github = """https://github.com/Antecer/QQChannelBot""";

	/// <summary>
	/// The copyright.
	/// </summary>
	public const string Copyright = """Copyright (c) 2021 Antecer. All rights reserved.""";

	/// <summary>
	/// The open-source SSH link.
	/// </summary>
	public const string GitSshLink = """git@github.com:Antecer/QQChannelBot.git""";


	/// <summary>
	/// Indicates the SDK assembly name.
	/// </summary>
	private static readonly AssemblyName Sdk = typeof(BotClient).Assembly.GetName();


	/// <summary>
	/// SDK name.
	/// </summary>
	public static string? SdkName => Sdk.Name;

	/// <summary>
	/// SDK version.
	/// </summary>
	public static Version? SdkVersion => Sdk.Version;
}
