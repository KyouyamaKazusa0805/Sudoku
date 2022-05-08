namespace Sudoku.Bot.Communication;

/// <summary>
/// SDK信息
/// </summary>
public static class InfoSdk
{
	/// <summary>
	/// Indicates the SDK assembly name.
	/// </summary>
	private static readonly AssemblyName Sdk = typeof(BotClient).Assembly.GetName();


	/// <summary>
	/// The open-source HTTPS link.
	/// </summary>
	public static string GitHTTPS => "https://github\uFEFF.com/Antecer/QQChannelBot";

	/// <summary>
	/// The open-source SSH link.
	/// </summary>
	public static string GitSSH => @"git@github.com:Antecer/QQChannelBot.git";

	/// <summary>
	/// The copyright.
	/// </summary>
	public static string Copyright => "Copyright © 2021 Antecer. All rights reserved.";

	/// <summary>
	/// SDK name.
	/// </summary>
	public static string? Name => Sdk.Name;

	/// <summary>
	/// SDK version.
	/// </summary>
	public static Version? Version => Sdk.Version;
}
