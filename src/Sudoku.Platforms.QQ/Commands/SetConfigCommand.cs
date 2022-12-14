namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines a set config command.
/// </summary>
[Command(Permissions.Owner)]
file sealed class SetConfigCommand : Command
{
	/// <inheritdoc/>
	public override string[] Prefixes => CommonCommandPrefixes.HashTag;

	/// <inheritdoc/>
	public override string CommandName => "set";

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (e is not { GroupId: var id } || BotRunningContext.GetContext(id) is not { Configuration: var config })
		{
			return true;
		}

		var argsSplit = args.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
		if (argsSplit is not [var rawPropertyName, var value])
		{
			return true;
		}

		switch (rawPropertyName.ToPascalCasing())
		{
			case nameof(UserDefinedConfigurations.RankingDisplayUsersCount) when int.TryParse(value, out var targetValue):
			{
				config.RankingDisplayUsersCount = targetValue;

				await e.SendMessageAsync(R["_MessageFormat_SetConfigSuccess"]!);
				break;
			}
		}

		return true;
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Converts the raw name like <c>prop-name-is-like-this</c> to pascal-casing one: <c>PropNameIsLikeThis</c>.
	/// </summary>
	/// <param name="this">The string identifier.</param>
	/// <returns>The converted value.</returns>
	public static string? ToPascalCasing(this string @this)
	{
		if (@this[0] == '-')
		{
			return null;
		}

		var indices = new List<int>();
		for (var i = @this.IndexOf('-'); i != -1; i = @this.IndexOf('-', i + 1))
		{
			indices.Add(i + 1);
		}

		switch (@this, indices)
		{
			case ([var firstChar, .. var lastChars], []):
			{
				return $"{char.ToUpper(firstChar)}{lastChars}";
			}
			case (_, { Count: not 0 }):
			{
				return new string(@this.Select((ch, i) => indices.Contains(i) || i == 0 ? char.ToUpper(ch) : ch).ToArray()).RemoveAll('-');
			}
			default:
			{
				return null;
			}
		}
	}
}
