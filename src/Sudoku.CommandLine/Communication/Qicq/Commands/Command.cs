namespace Sudoku.Communication.Qicq.Commands;

/// <summary>
/// Defines a command.
/// </summary>
internal abstract class Command
{
	/// <summary>
	/// Indicates the command name that will be used as comparison.
	/// </summary>
	public abstract string CommandName { get; }

	/// <summary>
	/// Indicates the environment command that the current command relies on.
	/// </summary>
	public virtual string? EnvironmentCommand { get; } = null;

	/// <summary>
	/// Indicates the prefix.
	/// </summary>
	public virtual string[] Prefixes { get; } = { "!", "\uff01" };

	/// <summary>
	/// Indicates the comparison mode that will be used as check commands.
	/// </summary>
	public abstract CommandComparison ComparisonMode { get; }


	/// <summary>
	/// Execute the command.
	/// </summary>
	/// <param name="args">The arguments.</param>
	/// <param name="e">The event arguments.</param>
	/// <returns>
	/// Returns a task instance that returns a <see cref="bool"/> value indicating whether the operation executed successfully.
	/// </returns>
	public async Task<bool> ExecuteAsync(string args, GroupMessageReceiver e)
	{
		var a = Prefixes.FirstOrDefault(args.StartsWith) switch
		{
			{ } prefix when args.IndexOf(prefix) is var i && i < args.Length => args[(i + 1)..],
			_ => null
		};

		if (a is null)
		{
			return false;
		}

		if (EnvironmentCommandExecuting != EnvironmentCommand)
		{
			return false;
		}

#pragma warning disable format
		if (ComparisonMode switch
			{
				CommandComparison.Strict => CommandName != a,
				CommandComparison.Prefix => !a.StartsWith(CommandName),
				_ => throw new ArgumentOutOfRangeException(nameof(ComparisonMode))
			})
#pragma warning restore format
		{
			return false;
		}

		return await ExecuteCoreAsync(
			ComparisonMode switch { CommandComparison.Strict => a, CommandComparison.Prefix => a[CommandName.Length..].Trim() },
			e
		);
	}

	/// <inheritdoc cref="ExecuteAsync(string, GroupMessageReceiver)"/>
	protected abstract Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e);
}
